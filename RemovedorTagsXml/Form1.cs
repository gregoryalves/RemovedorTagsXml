using RemovedorTagsXml.Business;
using RemovedorTagsXml.Interfaces;
using System;
using System.Windows.Forms;

namespace RemovedorTagsXml
{
    public partial class Form1 : Form
    {
        public IFormulario1Business FormularioBusiness;

        public Form1()
        {
            InitializeComponent();
            InitializeFormularioBusiness();
        }

        private void InitializeFormularioBusiness()
        {
            FormularioBusiness = new Formulario1Business(this);
        }

        private void BuscarDiretorioClick(object sender, EventArgs e)
        {
            FormularioBusiness.BuscarDiretorio();
        }

        private void ProcessarClick(object sender, EventArgs e)
        {
            FormularioBusiness.Processar();           
        }
    }
}
