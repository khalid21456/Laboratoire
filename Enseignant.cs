using System;
using System.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using Bunifu;
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using static Jenga.Theme;
using ServiceStack;
using System.Resources;
using MySql.Data;
using System.IO;
using ServiceStack.Model;

namespace FormAppNet
{
    public partial class Enseignant : Form
    {
        Connexion conn;
        private string email;
        private string password;
        public static int id;
        public string LastName;
        public int cardTop;
        private List<string> li = new List<string>();

        public Enseignant(string email, string password)
        {
            InitializeComponent();
            // changer le nom de host !
            conn = new Connexion(@"data source = BIGBOSS\SQLEXPRESS", "Labo");
            setEmail(email);
            setPassword(password);
            cardTop = 0;
        }

        public Enseignant()
        {
            InitializeComponent();
            conn = new Connexion(@"data source = BIGBOSS\SQLEXPRESS", "Labo");
            cardTop = 0;
        }

        private void Enseignant_Load(object sender, EventArgs e)
        {
            hideAll();
            accueil_ens1.Visible = true;
            logo.Visible = true;
            NomProfile.Visible = false;
            sidePicture.Visible = false;
            conn.ouvrirConnexion();

            SqlCommand cmd = conn.conx.CreateCommand();
            SqlCommand cmd2 = conn.conx.CreateCommand();
            SqlCommand cmd3 = conn.conx.CreateCommand();
            SqlCommand cmd4 = conn.conx.CreateCommand();
            cmd.CommandText = "SELECT Nom_ens, Prenom_ens, NbrRech, Specialite, dateEmbauch, reads, IdEns " +
                              "FROM Labo.dbo.Enseignant " +
                              "WHERE Email_ens = '"+getEmail()+"' AND MotPass_ens = '"+getPassword()+"'";
            SqlDataReader read = cmd.ExecuteReader();
            read.Read();
            LastName = read.GetString(1);
            accueil_ens1.nom.Text = read[0].ToString();
            accueil_ens1.prenom.Text = read[1].ToString();
            accueil_ens1.specialite.Text = read[3].ToString();
            accueil_ens1.nbrRecherche.Text = read[2].ToString();
            accueil_ens1.DateEmbauche.Text = read[4].ToString().Substring(0,10);
            NomProfile.Text = read[1].ToString() + " " + read[0].ToString();
            accueil_ens1.nbrReads.Text = read[5].ToString();
            id = Int32.Parse(read[6].ToString());
            read.Close();
            cmd2.CommandText = "SELECT COUNT(*) FROM Labo.dbo.Publication WHERE IdEns = " + id;
            cmd3.CommandText = "SELECT COUNT(*) FROM Labo.dbo.Evenement WHERE IdEns = " + id;
            cmd4.CommandText = "SELECT Description FROM Labo.dbo.projetRecherche WHERE IdEns = " + id;
            accueil_ens1.nbrPub.Text =  cmd2.ExecuteScalar().ToString();
            accueil_ens1.nbrEvents.Text = cmd3.ExecuteScalar().ToString();
            if(cmd4.ExecuteScalar() != null)
            {
                accueil_ens1.richTextBox1.Text = cmd4.ExecuteScalar().ToString();
            }else
            {
                accueil_ens1.richTextBox1.Text = "Aucun Activité";
            }
            
            
            conn.fermerConnexion();

            // changer le nom de compte !!!
            accueil_ens1.MainPicture.Image = new Bitmap("C:\\Users\\KHALID\\source\\repos\\FormAppNet\\FormAppNet\\Resources\\"+LastName+".png");
            this.sidePicture.Image = new Bitmap("C:\\Users\\KHALID\\source\\repos\\FormAppNet\\FormAppNet\\Resources\\" + LastName + ".png");
            publication_ens1.publicationPicture.Image = new Bitmap("C:\\Users\\KHALID\\source\\repos\\FormAppNet\\FormAppNet\\Resources\\" + LastName + ".png");

        }


        private void btnHome_Click(object sender, EventArgs e)
        {
            navigatedBar.Height = btnHome.Height;
            navigatedBar.Top = btnHome.Top;
            hideAll();
            accueil_ens1.Visible = true;
            logo.Visible = true;
            NomProfile.Visible = false;
            sidePicture.Visible = false;
        }

        private void btnEvents_Click(object sender, EventArgs e)
        {
            navigatedBar.Height = btnEvents.Height;
            navigatedBar.Top = btnEvents.Top;
            hideAll();
            evenements_ens1.Visible = true;
            logo.Visible = false;
            NomProfile.Visible = true;
            sidePicture.Visible = true;
            conn.ouvrirConnexion();

            SqlCommand cmd = conn.conx.CreateCommand();
            cmd.CommandText = "SELECT IdEns, Prenom_ens, Nom_ens FROM Labo.dbo.Enseignant";
            SqlDataReader read = cmd.ExecuteReader();
            evenements_ens1.ListeMembres.Items.Clear();
            while (read.Read())
            {
                if (Int32.Parse(read[0].ToString()) != id) {
                    evenements_ens1.ListeMembres.Items.Add(read[1].ToString() + " " + read[2].ToString());
                }
                
            }
            cmd.CommandText = "SELECT Prenom_doc, Nom_doc FROM Labo.dbo.Doctorant";
            read.Close();
            read = cmd.ExecuteReader();
            
            while(read.Read())
            {
                evenements_ens1.ListeMembres.Items.Add(read.GetString(0) + " " + read.GetString(1));
            }

            conn.fermerConnexion();

        }

        private void btnPub_Click(object sender, EventArgs e)
        {
            navigatedBar.Height = btnPub.Height;
            navigatedBar.Top = btnPub.Top;
            hideAll();
            publication_ens1.Visible = true;
            logo.Visible = false;
            NomProfile.Visible = true;
            sidePicture.Visible = true;
            conn.ouvrirConnexion();
            string req = "Select IdPub,Nom_ens, Prenom_ens, Date_pub, Specialite, Titre_pub, Contenu " +
                "from Labo.dbo.Enseignant inner join Labo.dbo.Publication on Labo.dbo.Publication.IdEns = Labo.dbo.Enseignant.IdEns ";
            SqlCommand cmd = conn.conx.CreateCommand();
            cmd.CommandText =  req;
                              
            SqlDataReader read = cmd.ExecuteReader();
            
            Bunifu.Framework.UI.BunifuCards card;
            
            
            while (read.Read())
            {
                if (!li.Contains(read[0].ToString()))
                {
                 li.Add(read[0].ToString());
                cardTop = cardTop + 200;
                card = new Bunifu.Framework.UI.BunifuCards();
                Label Nom = new Label();
                Label Date = new Label();
                Label Specialite = new Label();
                Label content = new Label();
                Label Titre = new Label();
                LinkLabel voir = new LinkLabel();
                voir.Text = "Voir+";
                PictureBox profile = new PictureBox();
                PictureBox pubPic = new PictureBox();
                Nom.Text =  read[2].ToString()+" "+ read[1].ToString();
                Date.Text = "Date : " + read[3].ToString().Substring(0,10);
                Specialite.Text = "Specialité : " + read[4].ToString();
                Titre.Text = read[5].ToString();
                content.Text = read[6].ToString();
                card.Width = 700;
                card.Height = 170;
                card.Top = cardTop;
                card.Left = 30;
                Nom.AutoSize = true;
                Date.AutoSize = true;
                Specialite.AutoSize = true;
                content.AutoSize = true;
                Titre.AutoSize = true;
                voir.AutoSize = true;
                card.color = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(228)))));
                card.Controls.Add(Nom);
                card.Controls.Add(Date);
                card.Controls.Add(Specialite);
                card.Controls.Add(Titre);
                card.Controls.Add(profile);
                card.Controls.Add(pubPic);
                card.Controls.Add(content);
                card.Controls.Add(voir);
                voir.Size = new System.Drawing.Size(41, 16);
                voir.TabStop = true;
                voir.Left = 600;
                voir.Top = 130;
                pubPic.Image = new Bitmap("C:\\Users\\KHALID\\source\\repos\\FormAppNet\\FormAppNet\\Resources\\Logo_fst.png");
                pubPic.Size = new System.Drawing.Size(160, 150);
                pubPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                Nom.Left = 230;
                Nom.Top = 20;
                Nom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                Date.Left = 230;
                Date.Top = 40;
                Specialite.Left = 230;
                Specialite.Top = 60;
                Titre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                Titre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(228)))));
                Titre.Left = 200;
                Titre.Top = 90;
                content.Left = 200;
                content.Top = 120;
                content.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                profile.Size = new System.Drawing.Size(50, 60);
                profile.Left=170;
                profile.Top = 20;
                profile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                profile.Image = new Bitmap("C:\\Users\\KHALID\\source\\repos\\FormAppNet\\FormAppNet\\Resources\\" + read[2].ToString() + ".png");
                publication_ens1.tabPublication.Controls.Add(card);
                }


            }

            conn.fermerConnexion();
        }

        private void btnDoc_Click(object sender, EventArgs e)
        {
            navigatedBar.Height = btnDoc.Height;
            navigatedBar.Top = btnDoc.Top;
            hideAll();
            doctorants2.Visible = true;
            logo.Visible = false;
            NomProfile.Visible = true;
            sidePicture.Visible = true;

            conn.ouvrirConnexion();
            SqlCommand cmd = conn.conx.CreateCommand();
            cmd.CommandText = "SELECT Nom_doc, Prenom_doc,Email_doc,projetRecherche.Titre_rech " +
                              "FROM Labo.dbo.Doctorant INNER JOIN Labo.dbo.projetRecherche " +
                              "ON Labo.dbo.projetRecherche.IdEns = Labo.dbo.Doctorant.IdEns " +
                              "WHERE Labo.dbo.Doctorant.IdEns = "+id;
            SqlDataReader read = cmd.ExecuteReader();
            doctorants2.dataGridView1.DataSource = null;
            doctorants2.dataGridView1.Columns.Clear();
            doctorants2.dataGridView1.ColumnCount = 5;
            doctorants2.dataGridView1.Columns[0].Name = "Nom";
            doctorants2.dataGridView1.Columns[1].Name = "Prénom";
            doctorants2.dataGridView1.Columns[2].Name = "Email";
            doctorants2.dataGridView1.Columns[3].Name = "Thèse";
            while (read.Read())
            {
                doctorants2.dataGridView1.Rows.Add(read[0], read[1], read[2], read[3]);
            }
            read.Close();
            conn.fermerConnexion();

        }

        private void btnDecnx_Click(object sender, EventArgs e)
        {
            // retourner vers l'inscription fenêtre
        }


        private void hideAll()
        {
            accueil_ens1.Visible = false;
            evenements_ens1.Visible = false;
            publication_ens1.Visible = false;
            doctorants2.Visible = false;
            Invitation_ens1.Visible = false;
        }

        private void home1_Load(object sender, EventArgs e)
        {

        }

        public void setEmail(string email)
        {
            this.email = email;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public string getEmail()
        {
            return email;
        }

        public string getPassword()
        {
            return password;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void btnInvit_Click(object sender, EventArgs e)
        {
            hideAll();
            navigatedBar.Height = btnInvit.Height;
            navigatedBar.Top = btnInvit.Top;
            Invitation_ens1.Visible = true;
            logo.Visible = false;
            NomProfile.Visible = true;
            sidePicture.Visible = true;
        }

        private void Invitation_ens1_Load(object sender, EventArgs e)
        {

        }
    }
}
