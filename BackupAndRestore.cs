using Npgsql;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Runtime.Loader;



namespace dba
{
    

    public static class BackupAndRestore
    {
        private static int backUpExitCode;
        private static int restoreExitCode;
        public static int ExecuteBackup(string password, string serverName, string portNumber, string username, string databaseName, string location, string fileName, string fileExtension)
        {
            
            

            Process p = new Process();
            p.StartInfo.FileName = "/bin/pg_dump";
            p.StartInfo.Arguments = $" \"postgresql://{username}:{password}@{serverName}:{portNumber}/{databaseName}\" -f {location}/{fileName}.{fileExtension}";	
            Console.WriteLine($"pg_dump \"postgresql://{username}:{password}@{serverName}:{portNumber}/{databaseName}\" > {location}/{fileName}.{fileExtension}");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.EnableRaisingEvents = true;
            Directory.SetCurrentDirectory("./");
            p.Exited += P_Exited_Backup;
            p.StartInfo.CreateNoWindow = true;
            try
            {
                p.Start();
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                string fNamestderr = "in" + "(stderr).txt";
                string fNamestdout = "out" + "(stdout).txt";
                StreamWriter stdin = p.StandardInput;
                //stdin.WriteLine(["Command Text"]);
                p.OutputDataReceived += (s, evt) => {
                    if (evt.Data != null)
                    {
                        using (StreamWriter sw = File.AppendText(fNamestdout))
                        {
                            sw.WriteLine(evt.Data);
                        }
                        Console.WriteLine(evt.Data);
                    }
                };
                p.ErrorDataReceived += (s, evt) => {
                    if (evt.Data != null)
                    {
                        using (StreamWriter sw = File.AppendText(fNamestderr))
                        {
                            sw.WriteLine(evt.Data);
                        }
                        Console.WriteLine(evt.Data);
                    }
                };
                p.WaitForExit();
                
              
                return backUpExitCode;
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1000;
            }

        }

       
        public static int ExecuteRestore(string password, string serverName, string portNumber, string username,
            string databaseName, string file)
        {
            Process p = new Process();
            p.StartInfo.FileName = "/bin/psql";
            //p.StartInfo.Arguments = "PGPASSWORD="+password+" pg_restore -h "+ serverName +" -p "+portNumber+" -U "+username+" -d "+ databaseName+" " + file + "";	
            //p.StartInfo.Arguments = "PGPASSWORD="+password+" psql -h "+ serverName +" -p "+portNumber+" -U "+username+" -d "+ databaseName+" -f " + file + "";
            p.StartInfo.Arguments =
                $" \"postgresql://{username}:{password}@{serverName}:{portNumber}/{databaseName}\" -f {file}";
            Console.WriteLine($"psql \"postgresql://{username}:{password}@{serverName}:{portNumber}/{databaseName}\" -f {file}");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName("./") ?? string.Empty;
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited_Restore;
            p.StartInfo.CreateNoWindow = false;

        
            try
            {
                p.Start();
                p.WaitForExit();
                p.Close();
                return restoreExitCode;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return 1000;
            }

            
        }
        
        private static void P_Exited_Restore(object sender, EventArgs e)
        {
            var p = sender as Process;
            restoreExitCode = p.ExitCode;
            Console.WriteLine($"P_Exited: Process Id {p.Id} exited with code: {p.ExitCode}");
        } 
        
        private static void P_Exited_Backup(object sender, EventArgs e)
        {
            var p = sender as Process;
            backUpExitCode = p.ExitCode;
            Console.WriteLine($"P_Exited: Process Id {p.Id} exited with code: {p.ExitCode}");
        } 
    }
}