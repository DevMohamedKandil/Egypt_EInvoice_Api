using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Egypt_EInvoice_Api.Models
{
	public class VWItem
	{
		[Key]
		public string Code { get; set; }
		public string InternalCode { get; set; }
		public string Name { get; set; }
		public string LatinName { get; set; }
		public string GS1Code { get; set; }
		public string EGSCode { get; set; }
		public string GPCCode { get; set; }
		public Guid? GroupGuid { get; set; }

	}
}
