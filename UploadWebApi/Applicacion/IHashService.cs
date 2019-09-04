using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWebApi.Applicacion
{
    public interface IHashService
    {

        byte[] CalcularHash(byte[] buffer);

    }
}
