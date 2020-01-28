/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 * Autor: Miguel A. Romera
 * Fecha: 27/01/2020 18:51:52
 */

using System;
using System.Collections.Generic;
using System.Linq;
using HuellaDactilarAceite.IndicesSimilitud;

namespace UploadWebApi.Aplicacion.Servicios.Procesadores.Imp
{

    /// <summary>
    /// Determina la forma en que los vectores se comparan entre
    /// ellos. 
    /// 
    /// </summary>
    public class ProcesadorVectorReferencia : IProcesadorVectores
    {
        const int VectorReferencia = 0;

        public ProcesadorVectorReferencia(IIndiceSimilitud similitud)
        {
            Indice = similitud;
        }

        public IIndiceSimilitud Indice { get; }

        /// <summary>
        /// El vector contenido en la posición 0, corresponde al vector referencia
        /// contra el que se comparan el resto de vectores.
        /// En el caso base, 2 vectores, en una comparación simple de dos vectores
        /// </summary>
        /// <param name="vectores"></param>
        /// <returns></returns>
        public IReadOnlyList<ItemIndice> ProcesarVectores(IEnumerable<ItemVector> vectores)
        {
            if (vectores.Count() < 2)
                throw new ArgumentException("El procesamiento de contraste de vectores debe poseer al menos 2 vectores");

            List<ItemIndice> indices = new List<ItemIndice>();

            for (var i = 1; i < vectores.Count(); i++)
                indices.Add(new ItemIndice(vectores.ElementAt(VectorReferencia).IdMuestra, vectores.ElementAt(i).IdMuestra, Indice.Similitud(vectores.ElementAt(VectorReferencia).Vector, vectores.ElementAt(i).Vector)));


            return indices.AsReadOnly();
        }
    }


}