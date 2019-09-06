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
using System.Web;
using Autofac;
using UploadWebApi.Infraestructura.Servicios;
using UploadWebApi.Infraestructura.Servicios.impl;

namespace UploadWebApi.Infraestructura.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    public class InfraestructuraModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.Register(c =>
            {
                return new IdentityService(HttpContext.Current?.GetOwinContext());

            }).As<IIdentityService>()
              .InstancePerRequest();
        }
    }
}