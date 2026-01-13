using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lector
{
    public partial class frmAgregarhuella : CaptureForm
    {
        public delegate void onTemplateEventHandler(DPFP.Template template);

        public event onTemplateEventHandler OnTemplate;
        private DPFP.Processing.Enrollment Enroller;
        protected override void Init()
        {
            
            base.Init();
            base.Text = "Dar alta huella";
            Enroller = new DPFP.Processing.Enrollment();
            UpdateStatus();
        }
        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) try
                {
                    MakeReport("The fingerprint feature set was created1.");
                    Enroller.AddFeatures(features);     // Add feature set to template.
                }
                finally
                {
                    UpdateStatus();

                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // report success and stop capturing
                            OnTemplate(Enroller.Template);
                            SetPrompt("Click Close, and then click Fingerprint Verification.");
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            Start();
                            break;
                    }
                }
        }
        private void UpdateStatus()
        {
            SetStatus(string.Format("Colocar el dedo el numero de veces {0}", Enroller.FeaturesNeeded));
        }
        public frmAgregarhuella()
        {
            InitializeComponent();
        }
    }
}
