using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Cursos;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>
    /// Administracion de cursos. Los preceptores y las autoridades crean y modifican; eliminar queda para las
    /// autoridades (lo controla el controlador) y solo si el curso no tiene alumnos ni materias.
    /// </summary>
    public class CursoService : ICursoService
    {
        private const int LargoMaximoAula = 50;
        private const int PrimerCicloValido = 2000;
        private const int CiclosFuturosPermitidos = 2;
        private const int MaximoAsignacionesPorVez = 200;

        private readonly ICursoRepository _cursos;
        private readonly IUsuarioRepository _usuarios;
        private readonly IMateriaRepository _materias;
        private readonly IUnitOfWork _unitOfWork;

        public CursoService(
            ICursoRepository cursos,
            IUsuarioRepository usuarios,
            IMateriaRepository materias,
            IUnitOfWork unitOfWork)
        {
            _cursos = cursos;
            _usuarios = usuarios;
            _materias = materias;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CursoDTO>> ListarAsync(Year? ano, Turno? turno)
        {
            var cursos = await _cursos.ListarAsync(ano, turno);
            return cursos.Select(c => c.ToDTO());
        }

        /// <inheritdoc />
        public async Task<CursoDTO> BuscarAsync(int id)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            return curso.ToDTO();
        }

        /// <inheritdoc />
        public async Task<CursoDTO> CrearAsync(CursoDTO dto)
        {
            ValidarValores(dto.Año, dto.Division, dto.Turno, dto.CicloLectivo);
            var aula = NormalizarAula(dto.Aula);

            if (await _cursos.ExisteAsync(dto.Año, dto.Division, dto.Turno, dto.CicloLectivo))
                throw new BadRequestException("Ya existe un curso con ese año, división, turno y ciclo lectivo.");

            var curso = new Curso
            {
                Año = dto.Año,
                Division = dto.Division,
                Turno = dto.Turno,
                Aula = aula,
                CicloLectivo = dto.CicloLectivo
            };

            await _cursos.AddAsync(curso);
            await _unitOfWork.SaveChangesAsync();

            return curso.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarCursoDTO dto)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            var ano = dto.Año ?? curso.Año;
            var division = dto.Division ?? curso.Division;
            var turno = dto.Turno ?? curso.Turno;
            var ciclo = dto.CicloLectivo ?? curso.CicloLectivo;

            ValidarValores(ano, division, turno, ciclo);

            // Solo si cambia la combinacion se busca un duplicado (si no, el curso se encontraria a si mismo).
            var cambioCombinacion = ano != curso.Año || division != curso.Division || turno != curso.Turno || ciclo != curso.CicloLectivo;
            if (cambioCombinacion && await _cursos.ExisteAsync(ano, division, turno, ciclo))
                throw new BadRequestException("Ya existe un curso con ese año, división, turno y ciclo lectivo.");

            curso.Año = ano;
            curso.Division = division;
            curso.Turno = turno;
            curso.CicloLectivo = ciclo;
            if (dto.Aula != null) curso.Aula = NormalizarAula(dto.Aula);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var curso = await _cursos.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el curso {id}.");

            var usuarios = (await _usuarios.ListarPorCursoAsync(id)).Count;
            var materias = (await _materias.ListarAsync(null, id)).Count;

            if (usuarios > 0 || materias > 0)
            {
                var partes = new List<string>();
                if (usuarios > 0) partes.Add(usuarios == 1 ? "1 usuario" : $"{usuarios} usuarios");
                if (materias > 0) partes.Add(materias == 1 ? "1 materia" : $"{materias} materias");

                throw new BadRequestException(
                    $"No se puede eliminar el curso: tiene {string.Join(" y ", partes)} asignados. Movelos a otro curso antes de eliminarlo.");
            }

            _cursos.Remove(curso);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<int> AsignarUsuariosAsync(int idCurso, IReadOnlyCollection<int> idUsuarios)
        {
            var ids = idUsuarios.Distinct().ToList();

            if (ids.Count == 0)
                throw new BadRequestException("Elegí al menos un alumno.");

            if (ids.Count > MaximoAsignacionesPorVez)
                throw new BadRequestException($"Se pueden asignar hasta {MaximoAsignacionesPorVez} alumnos por vez.");

            if (await _cursos.GetByIdAsync(idCurso) == null)
                throw new NotFoundException($"No se encontró el curso {idCurso}.");

            // Se valida todo antes de tocar nada: la asignacion es de todo o nada.
            var alumnos = new List<Usuario>();
            foreach (var id in ids)
            {
                var usuario = await _usuarios.GetByIdAsync(id)
                    ?? throw new BadRequestException($"El usuario {id} no existe.");

                if (!RolIds.EsAlumno(usuario.IdRol))
                    throw new BadRequestException($"{usuario.NombreUsuario} no es alumno: solo se asignan alumnos a un curso.");

                if (usuario.IdCurso != null)
                    throw new BadRequestException($"{usuario.NombreUsuario} ya tiene un curso asignado.");

                alumnos.Add(usuario);
            }

            foreach (var alumno in alumnos) alumno.IdCurso = idCurso;

            await _unitOfWork.SaveChangesAsync();

            return alumnos.Count;
        }

        private static void ValidarValores(Year ano, Division division, Turno turno, int cicloLectivo)
        {
            if (!Enum.IsDefined(ano)) throw new BadRequestException("El año del curso debe ser de 1º a 6º.");

            if (!Enum.IsDefined(division)) throw new BadRequestException("La división del curso debe ser de 1 a 12.");

            if (!Enum.IsDefined(turno)) throw new BadRequestException("El turno debe ser mañana, tarde o noche.");

            var ultimoCiclo = DateTime.UtcNow.Year + CiclosFuturosPermitidos;
            if (cicloLectivo < PrimerCicloValido || cicloLectivo > ultimoCiclo)
                throw new BadRequestException($"El ciclo lectivo debe estar entre {PrimerCicloValido} y {ultimoCiclo}.");
        }

        private static string? NormalizarAula(string? aula)
        {
            var limpia = aula?.Trim();

            if (string.IsNullOrEmpty(limpia)) return null;

            if (limpia.Length > LargoMaximoAula)
                throw new BadRequestException($"El aula no puede superar los {LargoMaximoAula} caracteres.");

            return limpia;
        }
    }
}
