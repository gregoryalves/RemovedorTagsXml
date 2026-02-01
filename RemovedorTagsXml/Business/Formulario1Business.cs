using RemovedorTagsXml.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RemovedorTagsXml.Business
{
    public class Formulario1Business : IFormulario1Business
    {
        private Form1 _formulario;
        private string _diretorio;
        private string _tag;
        private string[] _arquivos;
        private int _processados;

        public Formulario1Business(Form1 Formulario)
        {
            _formulario = Formulario;
        }

        public void BuscarDiretorio()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _formulario.TxtDiretorio.Text = dialog.SelectedPath;
                }
            }
        }

        public void Processar()
        {
            LimparLog();
            IniciarVariaveis();

            if (ValidarErros())
                return;

            BuscarArquivos();
            IniciarProgressBar();
            PercorrerArquivos();
            FinalizarLog();
        }

        private void FinalizarLog()
        {
            _formulario.TxtLog.AppendText(Environment.NewLine);
            _formulario.TxtLog.AppendText($"Processo finalizado. Arquivos alterados: {_processados}");
        }

        private void PercorrerArquivos()
        {
            foreach (var arquivo in _arquivos)
            {
                ProcessarArquivo(arquivo);
            }
        }

        private void ProcessarArquivo(string arquivo)
        {
            try
            {
                var documento = XDocument.Load(arquivo);
                var elementos = documento.Descendants().Where(x => x.Name.LocalName.Equals(_tag, StringComparison.OrdinalIgnoreCase)).ToList();
                if (elementos.Any())
                {
                    foreach (var elemento in elementos)
                    {
                        elemento.Remove();
                    }

                    documento.Save(arquivo);

                    _formulario.TxtLog.AppendText($"✔ Tag removida: {Path.GetFileName(arquivo)}{Environment.NewLine}");
                    _processados++;
                }
                else
                {
                    _formulario.TxtLog.AppendText($"ℹ Tag não encontrada: {Path.GetFileName(arquivo)}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                _formulario.TxtLog.AppendText($"❌ Erro em {Path.GetFileName(arquivo)}: {ex.Message}{Environment.NewLine}");
            }
            finally
            {
                _formulario.ProgressBar.Value++;
                Application.DoEvents();
            }
        }

        private void BuscarArquivos()
        {
            _arquivos = Directory.GetFiles(_diretorio, "*.xml");
        }

        private bool ValidarErros()
        {
            if (!Directory.Exists(_diretorio))
            {
                MessageBox.Show("Diretório inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            if (string.IsNullOrWhiteSpace(_tag))
            {
                MessageBox.Show("Informe o nome da tag.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            return false;
        }

        private void IniciarVariaveis()
        {
            _diretorio = _formulario.TxtDiretorio.Text.Trim();
            _tag = _formulario.TxtTag.Text.Trim();
            _processados = 0;
        }

        private void IniciarProgressBar()
        {
            _formulario.ProgressBar.Value = 0;
            _formulario.ProgressBar.Maximum = _arquivos.Length;
        }

        private void LimparLog()
        {
            _formulario.TxtLog.Clear();
        }
    }
}
