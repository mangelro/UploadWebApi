/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 12:40:18
 *
 */

using System;
using System.Configuration;
using System.Web;
using Autofac;

using UploadWebApi.Aplicacion.Mapeado;
using UploadWebApi.Aplicacion.Servicios;
using UploadWebApi.Aplicacion.Stores;
using UploadWebApi.Infraestructura.Datos;
using UploadWebApi.Infraestructura.Datos.Configuracion;
using UploadWebApi.Infraestructura.Mapeador;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Infraestructura.Configuracion;
using UploadWebApi.Aplicacion.Servicios.Imp;

namespace UploadWebApi.Infraestructura.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c =>
            {
                return new IdentityService(HttpContext.Current?.GetOwinContext());

            }).As<IIdentityService>()
           .InstancePerRequest();



            //Servicio de contraste
            builder.RegisterType<ContrasteHuellasService>()
                .As<IContrasteHuellasService>()
                .InstancePerRequest();

            //Servicio de gestion de huellas
            builder.RegisterType<GestionHuellasService>()
                .As<IGestionHuellasService>()
                .InstancePerRequest();


            builder.RegisterType<VectorReferenciaService>()
            .As<IVectorReferenciaService>()
            .InstancePerRequest();

            



            //Configuracion de los Stores
            builder.RegisterType<StoreConfiguration>()
                .As<IStoreConfiguration>()
                .InstancePerRequest();



            //Stores de huellas
            builder.RegisterType<DapperHuellaAceiteStore>()
                .As<IHuellaAceiteStore>()
                .InstancePerRequest();

         


            //Servicio de Funciones Hash
            builder.RegisterType<MD5HashService>()
                .As<IHashService>()
                .InstancePerRequest();

            //Configuración del almacenaje de registros
            builder.RegisterType<ConfiguracionRegistro>()
                .As<IConfiguracionRegistros>()
                .InstancePerRequest();


            //Servicio de Gestión de huellas
            builder.RegisterType<GestionHuellasService>()
            .AsSelf()
            .InstancePerRequest();


            //Servicio de Gestión de contrastes
            builder.RegisterType<ContrasteHuellasService>()
            .AsSelf()
            .InstancePerRequest();



            //Servicio de Gestión de contrastes
            builder.RegisterType<AutoMapperAdapter>()
               .As<IMapperService>()
               .InstancePerRequest();




        }


        string GetAppConfig(string configName, string defaultValue)
        {
            string val = ConfigurationManager.AppSettings[configName];

            return (String.IsNullOrEmpty(val) ? defaultValue : val);
        }
    }
}