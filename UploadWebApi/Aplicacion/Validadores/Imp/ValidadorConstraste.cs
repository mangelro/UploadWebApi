/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 28/01/2020 11:59:12
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuellaDactilarAceite.IndicesSimilitud;
using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Aplicacion.Servicios.Procesadores;
using UploadWebApi.Models;

namespace UploadWebApi.Aplicacion.Validadores.Imp
{
    /// <summary>
    /// 
    /// </summary>

    public class ValidadorContraste : IValidadorContraste
    {

        public ValidadorContraste(IIndiceSimilitud similitud)
        {
            Indice = similitud;
        }

        public IIndiceSimilitud Indice { get; }



        public bool ValidarConstraste(IEnumerable<ItemIndice> indices)
        {
            EstadoContraste estadoTemp = EstadoContraste.VALIDO;


            List<ItemIndice> listIndices = indices.ToList();
            for (var i=0;i< listIndices.Count;i++)
            {
                listIndices[i].Validez = (listIndices[i].Indice > Indice.UmbralAceptacion);

                if (listIndices[i].Validez)
                    estadoTemp = estadoTemp.Next(EstadoContraste.VALIDO);
                else
                    estadoTemp = estadoTemp.Next(EstadoContraste.INVALIDO);

            }


            return estadoTemp.EsValido;

        }
    }
}