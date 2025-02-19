﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.IO.Ports;
//using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Eolia_IHM.Properties
{
    internal class EoliaUtils
    {
        // Variable relatif a la gestion des onglets 
        
        static GroupBox ongletActif = null;

        // Variable relatif a la gestion du pavé numérique

        static TextBox textBoxActif = null;

        // Variable relatif a la gestion de la config

        static string DossierExecutable = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string FichierConfig = System.IO.Path.Combine(DossierExecutable, "config/EoliaConfig.conf");




        // Variable relatif a la gestion du pavé numérique

        public static void TextBoxActif(TextBox txtbox)
        {
            textBoxActif = txtbox;
        }

        public static void PaveNumerique(string x) // x = caractère a entré dans la textbox
        {                                // 10 = . / 11 = del
            if(textBoxActif!= null) {

                if (Int32.TryParse(x, out int result))
                {
                    textBoxActif.Text += result; 
                }else if(x == "Suppr")
                {
                    if (textBoxActif.Text.Length > 0)
                        textBoxActif.Text = textBoxActif.Text.Remove(textBoxActif.TextLength - 1);
                }
                else if(x == ".")
                {
                    textBoxActif.Text += x;
                }

                textBoxActif.Focus();
            }
        }


        // Fonction relatif a la gestion des onglets

        public static void AfficherOnglet(GroupBox gbox)
        {
            if (ongletActif == null)
            {
                gbox.Visible = true;
                ongletActif = gbox;
            }
            else if(ongletActif == gbox)
            {
                gbox.Visible = false;
                ongletActif = null;
            }
            else
            {
                ongletActif.Visible=false;
                gbox.Visible=true;
                ongletActif = gbox;
                
            }
        
        }



        // Fonction relatif a la sauvegarde et au chargement des fichiers de configuration

        public static void SauvegarderConfiguration(IDictionary<string, string> ListeValeurASauvegarder)
        {
           

            // Création d'un fichier de configuration
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Il faudra ajouter un foreach avec un dictionnaire de string
            // pour stocker plusieurs valeurs
            foreach(KeyValuePair<string, string> entry in ListeValeurASauvegarder)
            {
                config.AppSettings.Settings.Add(entry.Key, entry.Value);
            }
            

            // sauvegarde du fichier de configuration sous le nom "EoliaConfig.config"
            config.SaveAs(FichierConfig, ConfigurationSaveMode.Modified);

        }


        // Renvoi la valeur du champ "champ" du XML sous forme d'un String
        public static string LireConfiguration(string champ)
        {
            //permet de spécifier le chemin d'accès du fichier de configuration
            if (!EoliaConfigExiste()) return null;
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = FichierConfig
            };
            try
            {
                var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                //le configuration Userlevel.none ca permet simplement de signifier que c'est un fichier de configuration
                return config.AppSettings.Settings[champ].Value;
            }
            catch (Exception e)
            {
                MsgBoxNonBloquante("Un problème a eut lieu avec le fichier de configuration");
                File.Delete(FichierConfig);
                EoliaLogs.Write(e.Message, EoliaLogs.Types.ERROR);
                return null;
            }

        }


        // Fonction utilitaire

        public static void AfficherPortSerie(ComboBox cmbBox)
        {

            cmbBox.Items.Clear();

            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                cmbBox.Items.Add(port);
            }


        }

        public static bool EoliaConfigExiste()
        {
            //if (!Directory.Exists("config")) Directory.CreateDirectory("config");
            if (File.Exists(FichierConfig))
            {
                return true;
            }

            return false;
        }

        public static void MsgBoxNonBloquante(string Msg, string title = null)
        {
            EoliaLogs.Write("Message affiché : " + Msg, EoliaLogs.Types.DEBUG);
            Task.Run(() => {
                if (title != null) MessageBox.Show(Msg, title);
                else MessageBox.Show(Msg);

            });
        
        }

    }
}
