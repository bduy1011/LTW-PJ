using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_Photo_Editor
{
    public partial class workTabControl : Component
    {
        public workTabControl()
        {
            InitializeComponent();
        }

        public workTabControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
