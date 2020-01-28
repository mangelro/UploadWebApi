/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 * Autor: Miguel A. Romera
 * Fecha: 27/01/2020 18:50:00
 */

using System.Collections.Generic;

using HuellaDactilarAceite.IndicesSimilitud;
using HuellaDactilarAceite.Modelo;

namespace UploadWebApi.Aplicacion.Servicios.Procesadores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProcesadorVectores
    {
        /// <summary>
        /// Indice utilziado para el procesar los vectores
        /// </summary>
        IIndiceSimilitud Indice { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectores"></param>
        /// <returns></returns>
        IReadOnlyList<ItemIndice> ProcesarVectores(IEnumerable<ItemVector> vectores); 
    }


    public class ItemVector
    {

        public string IdMuestra { get; }

        public VectorHuellaAceite Vector { get; }

        public ItemVector(string idMuestra, VectorHuellaAceite vector)
        {
            IdMuestra = idMuestra;
            Vector = vector;
        }    

    }

    public class ItemIndice { 

        public string IdMuestra1 { get; }

        public string IdMuestra2 { get; }

        public IndiceSimilitud Indice { get; }

        public bool Validez { get; set; } 

        public ItemIndice(string muestra1, string muestra2, IndiceSimilitud indice)
        {
            IdMuestra1 = muestra1;
            IdMuestra2 = muestra2;
            Indice = indice;
            Validez = true;
        }
    }

}