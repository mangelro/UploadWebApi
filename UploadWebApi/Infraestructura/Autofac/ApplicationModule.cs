/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 05/09/2019 12:40:18
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using UploadWebApi.Applicacion;
using UploadWebApi.Applicacion.Servicios;
using UploadWebApi.Applicacion.Stores;

namespace UploadWebApi.Infraestructura.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Configuracion de los Stores
            builder.RegisterType<StoreConfiguration>()
                .As<IStoreConfiguration>()
                .InstancePerRequest();

            //Stores de huellas
            builder.RegisterType<DapperHuellasStore>()
                .As<IHuellasStore>()
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



            

        }
    }
}