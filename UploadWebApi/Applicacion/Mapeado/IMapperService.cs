using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Applicacion.Mapeado
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



        IEnumerable<TDto> Map<TDomain, TDto>(IEnumerable<TDomain> domain) where TDto : class;

    }
}
