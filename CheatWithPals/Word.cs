//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CheatWithPals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public partial class Word
    {
        public int Id { get; set; }
        public string Word1 { get; set; }
        public int Point { get; set; } // manually added
        public string Def { get; set; }
        public string AudioLink
        {
            get
            {
                if (!string.IsNullOrEmpty(Def) && !this.Def.Contains("{wav}"))
                {

                    return string.Format("http://media.merriam-webster.com/soundc11/{0}/{1}", Word1.ElementAt(0).ToString() ,this.Def.Split(new char[] { ';' }).First());
                }
                return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(this.Def))
                {
                    Def = value + ";{Def}";
                }
                else if (Def.Contains("{wav}"))
                {
                    this.Def.Replace("{wav}", value);
                }
            }
        }
        public string Definition
        {
            get
            {
                if (!string.IsNullOrEmpty(Def) && !this.Def.Contains("{def}" ))
                {
                    return this.Def.Split(new char[] { ';' }).ElementAt(1);
                }
                return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(this.Def))
                {
                    Def = "{wav};" + value;
                }
                else if (Def.Contains("{def}"))
                {
                    this.Def.Replace("{def}", value);
                }
            }
        }
    }
}
