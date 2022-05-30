using System;
using Gtk;
using Npgsql;
using UI = Gtk.Builder.ObjectAttribute;

namespace dba
{
    class MainWindow : Window
    {
        
        [UI] private Entry host = null;
        [UI] private Entry port = null;
        [UI] private Entry username = null;
        [UI] private Entry password = null;
        [UI] private Entry database = null;
        [UI] private Button connect = null;

        [UI] private Button execute_backup = null;
        [UI] private Button execute_restore = null;

        [UI] private FileChooserButton select_backup = null;
        [UI] private FileChooserButton select_restore = null;

        [UI] private Entry backup_loc = null;
        [UI] private Entry restore_loc = null;

        [UI] private Entry fileName = null;
        [UI] private ComboBoxText fileType = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            execute_backup.Sensitive = false;
            execute_restore.Sensitive = false;
            backup_loc.Sensitive = false;
            restore_loc.Sensitive = false;
            DeleteEvent += Window_DeleteEvent;
            connect.Clicked += Boutton_Connect_Clicked;
            select_backup.SelectionChanged += Backup_Chooser;
            select_restore.SelectionChanged += Restore_Chooser;
            execute_backup.Clicked += BackupExecute;
            execute_restore.Clicked += RestoreExecute;
            /*host.Text = "127.0.0.1";
            port.Text = "5432";
            username.Text = "postgres";
            password.Text = "123456";
            database.Text = "bizerte1";*/
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
        

        private void Boutton_Connect_Clicked(object sender, EventArgs a)
        {
                
            
            Console.WriteLine("Clicked");
            Console.WriteLine(host.Text);
            Console.WriteLine(port.Text);
            Console.WriteLine(username.Text);
            Console.WriteLine(password.Text);
            Console.WriteLine(database.Text);



            if (host.Text.Length == 0 || port.Text.Length == 0 || username.Text.Length == 0 || password.Text.Length == 0 ||
                database.Text.Length == 0)
            {
                DialogWindow win = new DialogWindow("Attention", $"Veuillez remplir le formulaire de connexion.", "Alerte");
                win.Show();
            }
            else
            {
                string strConnString = "Server=" + host.Text + ";Port=" + port.Text + ";User Id=" + username.Text + ";Password=" +
                                       password.Text + ";Database=" + database.Text + "";

                try
                {
                    NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
                    objConn.Open();
                    objConn.Close();
                    Console.WriteLine("Connected");
                    execute_backup.Sensitive = true;
                    execute_restore.Sensitive = true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    Console.WriteLine(ex.Data["MessageText"]);
                    DialogWindow win = new DialogWindow("Erreur", ex.Data["MessageText"].ToString(), "Erreur de connexion");
                    win.Show();
                
                }
            }
            
        }

        private void Backup_Chooser(object sender, EventArgs e)
        {
            Console.WriteLine(select_backup.CurrentFolder);
            backup_loc.Text = select_backup.CurrentFolder;
        }

        private void BackupExecute(object sender, EventArgs e)
        {
            Console.WriteLine(fileName.Text);
            Console.WriteLine(fileType.ActiveText);

            if (backup_loc.Text.Length == 0)
            {
                DialogWindow win = new DialogWindow("Attention", $"Veuillez séléctionner un dossier.", "Alerte");
                win.Show();
            }
            else if (fileName.Text.Length == 0 || fileType.ActiveText.Length == 0)
            {
                DialogWindow win = new DialogWindow("Attention", $"Veuillez saisir le nom du fichier et choisir une extension.", "Alerte");
                win.Show();
            }
            else
            {
                ExecutionWindow win1 = new ExecutionWindow();
                win1.Show();
                int statusCode = BackupAndRestore.ExecuteBackup(password.Text, host.Text, port.Text, username.Text, database.Text, select_backup.CurrentFolder, fileName.Text, fileType.ActiveText);
                win1.Hide();
                if (statusCode == 0)
                {
                    DialogWindow win = new DialogWindow("Succès", $"La base de données {database.Text} a été sauvegardée avec succès", "Succès de sauvegarde");
                    win.Show();
                } else
                {
                    DialogWindow win = new DialogWindow("Erreur", $"Erreur lors de la sauvegarde de la base de données {database.Text}, veuillez réssayer utlérieurement.", "Erreur de sauvegarde");
                    win.Show();
                }
            }
        }

        private void Restore_Chooser(object sender, EventArgs e)
        {
            Console.WriteLine(select_restore.CurrentFolder);
            restore_loc.Text = select_restore.Filename;
        }

        private void RestoreExecute(object sender, EventArgs e)
        {
            if (restore_loc.Text.Length == 0)
            {
                DialogWindow win = new DialogWindow("Attention", $"Veuillez séléctionner un fichier.", "Alerte");
                win.Show();
                
            }
            else
            {
                

                int statusCode = BackupAndRestore.ExecuteRestore(password.Text, host.Text, port.Text, username.Text, database.Text, select_restore.Filename);
                if (statusCode == 0)
                {
                    DialogWindow win = new DialogWindow("Succès", $"La base de données {database.Text} a été restaurée avec succès.", "Succès de restauration");
                    win.Show();
                } else
                {
                    DialogWindow win = new DialogWindow("Erreur", $"Erreur lors de la restauration de la base de données {database.Text}, veuillez réssayer utlérieurement.", "Erreur de restauration");
                    win.Show();
                }
            }
        }
    }
}
