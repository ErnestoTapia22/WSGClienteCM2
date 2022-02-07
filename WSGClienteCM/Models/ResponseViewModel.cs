﻿using System;
using System.Collections.Generic;

namespace WSGClienteCM.Models
{
    public class ResponseViewModel
    {
        public string P_COD_ERR { get; set; }
       
        public string P_NCODE { get; set; }
        public string P_MESSAGE { get; set; }
        public string P_SMESSAGE { get; set; }
        public string P_CAMPO { get; set; }
        public string P_SEMAIL { get; set; }
        public string P_SCOD_CLIENT { get; set; }
        public string P_SURL_SISTEMA { get; set; }
        public string P_NIDCM { get; set; }
        public List<ClientViewModel> EListClient { get; set; }
        public List<ListViewErrores> EListErrores { get; set; }
        public List<DetailBindingModel> ElistDetail { get; set; }
        public Object Data { get; set; }
    }

    public class Cabecera
    {
        public string P_COD_ERR { get; set; }

        public string P_NCODE { get; set; }
        public string P_MESSAGE { get; set; }
        public string P_SMESSAGE { get; set; }
        public string SCODAPLICACION { get; set; }
        public Int64 NUSERCODE { get; set; }
        public string SNROPROCESO_CAB { get; set; }
        public string SFILENAME { get; set; }

        public Decimal? NNROPROCESO_DET { get; set; }
        public string SIDDOC { get; set; }  
        public List<DetailBindingModel> ElistDetail { get; set; } 
    }
}
