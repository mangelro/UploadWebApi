/*
 * Copyright © 2020 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 28/01/2020 12:03:45
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Aplicacion.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    public class EstadoContraste
    {

        public readonly static EstadoContraste VALIDO = new EstadoContraste(Estados.VALIDO);
        public readonly static EstadoContraste INVALIDO = new EstadoContraste(Estados.INVALIDO);
        private readonly static EstadoContraste PREINVALIDO1 = new EstadoContraste(Estados.PREINVALIDO1);
        private readonly static EstadoContraste PREINVALIDO2 = new EstadoContraste(Estados.PREINVALIDO2);
        private readonly static EstadoContraste PREINVALIDO3 = new EstadoContraste(Estados.PREINVALIDO3);
        private readonly static EstadoContraste PREINVALIDO4 = new EstadoContraste(Estados.PREINVALIDO4);
        private enum Estados
        {
            VALIDO,
            PREINVALIDO1,
            PREINVALIDO2,
            PREINVALIDO3,
            PREINVALIDO4,
            INVALIDO,
        }


        readonly Estados _estado;
        private EstadoContraste(Estados estado)
        {
            _estado= estado;
        }


        public EstadoContraste Next(EstadoContraste nuevoEstado)
        {

            //if (nuevoEstado == EstadoContraste.VALIDO)
            //{
            //    if (this == EstadoContraste.PREINVALIDO)
            //        return EstadoContraste.VALIDO;

            //}
            //else 

            if (nuevoEstado == EstadoContraste.VALIDO)
            {
                if (this == EstadoContraste.PREINVALIDO4)
                    return EstadoContraste.PREINVALIDO3;
                else if (this == EstadoContraste.PREINVALIDO3)
                    return EstadoContraste.PREINVALIDO2;
                else if (this == EstadoContraste.PREINVALIDO2)
                    return EstadoContraste.PREINVALIDO1;
            }
            else if (nuevoEstado == EstadoContraste.INVALIDO)
            {
                if (this == EstadoContraste.VALIDO)
                    return EstadoContraste.PREINVALIDO3;
                else if (this == EstadoContraste.PREINVALIDO1)
                    return EstadoContraste.PREINVALIDO2;
                else if (this == EstadoContraste.PREINVALIDO2)
                    return EstadoContraste.PREINVALIDO3;
                else if (this == EstadoContraste.PREINVALIDO3)
                    return EstadoContraste.PREINVALIDO4;
                else if (this == EstadoContraste.PREINVALIDO4)
                    return EstadoContraste.INVALIDO;
            }

            return this;
        }


        public bool EsValido
        {
            get
            {

                if (this == EstadoContraste.INVALIDO)
                    return false;

                if (this == EstadoContraste.PREINVALIDO2)
                    return false;


                if (this == EstadoContraste.PREINVALIDO3)
                    return false;

                return true;
            }
        } 


        public override bool Equals(object obj)
        {

            EstadoContraste otroEstado;

            if ((otroEstado = obj as EstadoContraste) == null)
                return false;


            return _estado == otroEstado._estado;
        }

        public override int GetHashCode()
        {
            return _estado.GetHashCode();
        }

    }
}