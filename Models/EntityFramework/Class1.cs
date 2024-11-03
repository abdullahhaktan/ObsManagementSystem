using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObsSistem.Models.EntityFramework
{
    public partial class myViewModel
    {
        public List<AkademisyenModel> akademisyenList { get; set; }
        public List<bolumModel> bolumList { get; set; }
    }

    public partial class bolumListModel
    {
        public List<bolumModel> bolumList { get; set; }
    }
}