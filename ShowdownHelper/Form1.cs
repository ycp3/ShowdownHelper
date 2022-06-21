using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Web;
using CefSharp.WinForms;
using System.Text.Json;
using System.Windows.Forms.VisualStyles;

namespace ShowdownHelper
{
    public partial class Form1 : Form
    {
        private static readonly string[] types =
        {
            "Bug", "Dark", "Dragon", "Electric", "Fairy", "Fighting", "Fire",
            "Flying", "Ghost", "Grass", "Ground", "Ice", "Normal", "Poison",
            "Psychic", "Rock", "Steel", "Water"
        };

        private string p1;
        private string p2;

        private Dictionary<string, Pokedex.Pokemon> pokedex;

        private Dictionary<string, Dictionary<string, Dictionary<string, int>>> typechart;
        
        public Form1()
        {
            InitializeComponent();

            p1 = "";
            p2 = "";

            pokedex = JsonSerializer.Deserialize<Dictionary<string, Pokedex.Pokemon>>(File.ReadAllText(@"C:\Users\Jasper\RiderProjects\ShowdownHelper\ShowdownHelper\pokedex.txt"));
            typechart = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(
                File.ReadAllText(@"C:\Users\Jasper\RiderProjects\ShowdownHelper\ShowdownHelper\typechart.txt"));
            
            InitializeChromium();
        }

        private void InitializeChromium()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            Cef.Initialize(settings);
            
            var browser = new ChromiumWebBrowser("https://pokemonshowdown.com/");
            browser.Location = Point.Empty;
            browser.Anchor = AnchorStyles.Left;
            browser.Size = new Size((int)(ClientSize.Width * 0.8), ClientSize.Height);
            browser.Name = "browser";

            browser.ConsoleMessage += browser_ConsoleMessage;

            var output = new Label();

            output.Anchor = AnchorStyles.Right;
            output.Size = new Size(ClientSize.Width - browser.Size.Width, ClientSize.Height);
            output.Location = new Point(browser.Size.Width, 0);
            output.Name = "output";


            Controls.Add(browser);
            Controls.Add(output);
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void UpdateTypes()
        {
            if (p1 == "" || p2 == "") return;

            var data = new List<Tuple<string, string, List<string>>>();
            List<string> p1types = pokedex[p1.ToLower().Replace("-", "")].types;
            List<string> p2types = pokedex[p2.ToLower().Replace("-", "")].types;
            data.Add(new Tuple<string, string, List<string>>("P1: ", p1, p1types));
            data.Add(new Tuple<string, string, List<string>>("P2: ", p2, p2types));

            string str = "";
            
            foreach (var player in data)
            {
                str += player.Item1 + player.Item2 + "\n";

                foreach (var type in player.Item3)
                {
                    str += type + " ";
                }

                Dictionary<string, int> eff = new Dictionary<string, int>();

                foreach (var type in types)
                {
                    int typeeff = 0;
                    foreach (var playertype in player.Item3)
                    {
                        int damageTaken = typechart[playertype.ToLower()]["damageTaken"][type];
                        
                        if (damageTaken == 3)
                        {
                            typeeff = -3;
                            break;
                        }
                        
                        if (damageTaken == 2)
                        {
                            typeeff--;
                        }
                        else if (damageTaken == 1)
                        {
                            typeeff++;
                        }
                    }
                    eff.Add(type, typeeff);
                }

                var output = new List<Tuple<string, List<string>>>();
                output.Add(new Tuple<string, List<string>>("4", new List<string>()));
                output.Add(new Tuple<string, List<string>>("2", new List<string>()));
                output.Add(new Tuple<string, List<string>>("1", new List<string>()));
                output.Add(new Tuple<string, List<string>>("0.5", new List<string>()));
                output.Add(new Tuple<string, List<string>>("0.25", new List<string>()));
                output.Add(new Tuple<string, List<string>>("0", new List<string>()));

                foreach (var item in eff)
                {
                    output[2 - item.Value].Item2.Add(item.Key);
                }

                foreach (var item in output)
                {
                    if (!item.Item2.Any()) continue;

                    str += "\ntakes " + item.Item1 + "x damage from:\n";
                    foreach (var type in item.Item2)
                    {
                        str += type + "\n";
                    }
                }

                str += "\n\n";
            }

            Controls["output"].Invoke((Action) delegate
            {
                Controls["output"].Text = str;
            });
        }

        private void browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Match match1 = Regex.Match(e.Message, @"\|switch\|p1a: \w+\|(.+?),");
            Match match2 = Regex.Match(e.Message, @"\|switch\|p2a: \w+\|(.+?),");

            if (match1.Success)
            {
                p1 = match1.Groups[1].Value;
            }
            
            if (match2.Success)
            {
                p2 = match2.Groups[1].Value;
            }
            
            UpdateTypes();
        }
    }
}