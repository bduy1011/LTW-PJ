using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_Photo_Editor
{
    public partial class WorkTabControl : Component
    {
        public WorkTabControl()
        {
            InitializeComponent();
        }

        public WorkTabControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
