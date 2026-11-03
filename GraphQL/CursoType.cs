using HotChocolate.Types;
using Plataforma24API.DTOs.Cursos;

namespace Plataforma24API.GraphQL
{
    /// <summary>
    /// Esquema GraphQL de <see cref="CursoDTO"/>. Los nombres de campo de GraphQL no admiten la letra "ñ",
    /// por eso la propiedad <c>Año</c> se publica como <c>anio</c>.
    /// </summary>
    public class CursoType : ObjectType<CursoDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<CursoDTO> descriptor)
        {
            descriptor.Field(curso => curso.Año).Name("anio");
        }
    }
}
