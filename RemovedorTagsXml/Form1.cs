using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RemovedorTagsXml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDiretorio.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnProcessar_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            progressBar1.Value = 0;

            string diretorio = txtDiretorio.Text.Trim();
            string tag = txtTag.Text.Trim();

            if (!Directory.Exists(diretorio))
            {
                MessageBox.Show("Diretório inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                MessageBox.Show("Informe o nome da tag.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var arquivos = Directory.GetFiles(diretorio, "*.xml");

            progressBar1.Maximum = arquivos.Length;

            int processados = 0;

            foreach (var arquivo in arquivos)
            {
                try
                {
                    var doc = XDocument.Load(arquivo);

                    var elementos = doc.Descendants()
                        .Where(x => x.Name.LocalName.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (elementos.Any())
                    {
                        foreach (var el in elementos)
                            el.Remove();

                        doc.Save(arquivo);
                        txtLog.AppendText($"✔ Tag removida: {Path.GetFileName(arquivo)}{Environment.NewLine}");
                        processados++;
                    }
                    else
                    {
                        txtLog.AppendText($"ℹ Tag não encontrada: {Path.GetFileName(arquivo)}{Environment.NewLine}");
                    }
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"❌ Erro em {Path.GetFileName(arquivo)}: {ex.Message}{Environment.NewLine}");
                }
                finally
                {
                    progressBar1.Value++;
                    Application.DoEvents(); // atualiza a UI
                }
            }

            txtLog.AppendText(Environment.NewLine);
            txtLog.AppendText($"Processo finalizado. Arquivos alterados: {processados}");
        }

    }
}
