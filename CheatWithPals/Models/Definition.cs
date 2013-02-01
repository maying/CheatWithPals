using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheatWithPals.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class entry_list
    {

        private entry_listEntry entryField;

        private decimal versionField;

        /// <remarks/>
        public entry_listEntry entry
        {
            get
            {
                return this.entryField;
            }
            set
            {
                this.entryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntry
    {

        private string ewField;

        private string hwField;

        private entry_listEntrySound soundField;

        private string prField;

        private string flField;

        private entry_listEntryET etField;

        private entry_listEntryDef defField;

        private entry_listEntryUro uroField;

        private string idField;

        /// <remarks/>
        public string ew
        {
            get
            {
                return this.ewField;
            }
            set
            {
                this.ewField = value;
            }
        }

        /// <remarks/>
        public string hw
        {
            get
            {
                return this.hwField;
            }
            set
            {
                this.hwField = value;
            }
        }

        /// <remarks/>
        public entry_listEntrySound sound
        {
            get
            {
                return this.soundField;
            }
            set
            {
                this.soundField = value;
            }
        }

        /// <remarks/>
        public string pr
        {
            get
            {
                return this.prField;
            }
            set
            {
                this.prField = value;
            }
        }

        /// <remarks/>
        public string fl
        {
            get
            {
                return this.flField;
            }
            set
            {
                this.flField = value;
            }
        }

        /// <remarks/>
        public entry_listEntryET et
        {
            get
            {
                return this.etField;
            }
            set
            {
                this.etField = value;
            }
        }

        /// <remarks/>
        public entry_listEntryDef def
        {
            get
            {
                return this.defField;
            }
            set
            {
                this.defField = value;
            }
        }

        /// <remarks/>
        public entry_listEntryUro uro
        {
            get
            {
                return this.uroField;
            }
            set
            {
                this.uroField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntrySound
    {

        private string wavField;

        /// <remarks/>
        public string wav
        {
            get
            {
                return this.wavField;
            }
            set
            {
                this.wavField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntryET
    {

        private string[] itField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("it")]
        public string[] it
        {
            get
            {
                return this.itField;
            }
            set
            {
                this.itField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntryDef
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("date", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("dt", typeof(entry_listEntryDefDT))]
        [System.Xml.Serialization.XmlElementAttribute("sn", typeof(byte))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntryDefDT
    {

        private string d_linkField;

        private string[] textField;

        /// <remarks/>
        public string d_link
        {
            get
            {
                return this.d_linkField;
            }
            set
            {
                this.d_linkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class entry_listEntryUro
    {

        private string ureField;

        private string flField;

        /// <remarks/>
        public string ure
        {
            get
            {
                return this.ureField;
            }
            set
            {
                this.ureField = value;
            }
        }

        /// <remarks/>
        public string fl
        {
            get
            {
                return this.flField;
            }
            set
            {
                this.flField = value;
            }
        }
    }


}