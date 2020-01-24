/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 12:40:18
 *
 */

using Autofac;

using UploadWebApi.Infraestructura.Compresion;

namespace UploadWebApi.Infraestructura.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    public class InfraestructuraModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {

         

            //filtro para comprimir huellas --FakeCompresionService/ GZipCompresionService / SevenZipCompresionService
            //builder.RegisterType<GZipCompresionService>()
            //    .Keyed<IFiltroCompresion>(CompresionServiceType.GZipCompresionService)
            //    .InstancePerRequest();

            //builder.RegisterType<FakeCompresionService>()
            //.Keyed<IFiltroCompresion>(CompresionServiceType.FakeCompresionService)
            //.InstancePerRequest();

            //builder.RegisterType<SevenZipCompresionService>()
            //.Keyed<IFiltroCompresion>(CompresionServiceType.SevenZipCompresionService)
            //.InstancePerRequest();

            //builder.Register(c =>
            //{
            //    CompresionServiceType compresion = (CompresionServiceType) Enum.Parse(typeof(CompresionServiceType), GetAppConfig("storeConfCompresionService", CompresionServiceType.GZipCompresionService.ToString()));

            //    return new DapperHuellasStore(c.Resolve<IStoreConfiguration>(), 
            //                            c.ResolveKeyed<IFiltroCompresion>(compresion));
            //})
            //.As<IHuellasStore>()
            //.InstancePerRequest();


            //filtro para comprimir huellas --FakeCompresionService/ GZipCompresionService / SevenZipCompresionService
            builder.RegisterType<GZipCompresionService>()
            .As<IFiltroCompresion>()
            .InstancePerRequest();

        }
    }
}