using System;


namespace UploadWebApi.Aplicacion.Mapeado
{
    public interface IMapperService
    {

        /// <summary>
        /// Mapea una entidad del dominio en un dto
        /// </summary>
        /// <typeparam name="TDomain">
        /// Tipo de entidad del dominio
        /// </typeparam>
        /// <typeparam name="TItem">
        /// Tipo de entidad Dto
        /// </typeparam>
        /// <param name="domain">
        /// Entidad del dominio
        /// </param>
        /// <returns>
        /// item mapeado
        /// </returns>
        TDto Map<TDomain, TDto>(TDomain domain) where TDto : class;

    }
}
