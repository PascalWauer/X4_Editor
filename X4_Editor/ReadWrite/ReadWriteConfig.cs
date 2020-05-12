using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X4_Editor
{
    public class ReadWriteConfig
    {
        private UIManager m_UIManager;
        public ReadWriteConfig(UIManager uiManager)
        {
            m_UIManager = uiManager;
        }
        public void SaveConfig()
        {
            string path = Environment.CurrentDirectory;

            using (StreamWriter sw = new StreamWriter(path + "\\X4_Editor.cfg"))
            {
                if (Directory.Exists(m_UIManager.UIModel.Path))
                    sw.WriteLine("Vanilla X4 Path: " + m_UIManager.UIModel.Path);
                else
                    sw.WriteLine("Vanilla X4 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath1))
                    sw.WriteLine("Mod 1 Path: " + m_UIManager.UIModel.ModPath1);
                else
                    sw.WriteLine("Mod 1 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath2))
                    sw.WriteLine("Mod 2 Path: " + m_UIManager.UIModel.ModPath2);
                else
                    sw.WriteLine("Mod 2 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath3))
                    sw.WriteLine("Mod 3 Path: " + m_UIManager.UIModel.ModPath3);
                else
                    sw.WriteLine("Mod 3 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath4))
                    sw.WriteLine("Mod 4 Path: " + m_UIManager.UIModel.ModPath4);
                else
                    sw.WriteLine("Mod 4 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath5))
                    sw.WriteLine("Mod 5 Path: " + m_UIManager.UIModel.ModPath5);
                else
                    sw.WriteLine("Mod 5 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ModPath6))
                    sw.WriteLine("Mod 6 Path: " + m_UIManager.UIModel.ModPath6);
                else
                    sw.WriteLine("Mod 6 Path: ");
                if (Directory.Exists(m_UIManager.UIModel.ExportPath))
                    sw.WriteLine("Mod export Path: " + m_UIManager.UIModel.ExportPath);
                else
                    sw.WriteLine("Mod export Path: ");

                // filters
                if (m_UIManager.UIModel.Ships)
                    sw.WriteLine("Ships = 1");
                else
                    sw.WriteLine("Ships = 0");
                if (m_UIManager.UIModel.Shields)
                    sw.WriteLine("Shields = 1");
                else
                    sw.WriteLine("Shields = 0");
                if (m_UIManager.UIModel.Engines)
                    sw.WriteLine("Engines = 1");
                else
                    sw.WriteLine("Engines = 0");
                if (m_UIManager.UIModel.Weapons)
                    sw.WriteLine("Weapons = 1");
                else
                    sw.WriteLine("Weapons = 0");
                if (m_UIManager.UIModel.Wares)
                    sw.WriteLine("Wares = 1");
                else
                    sw.WriteLine("Wares = 0");
                if (m_UIManager.UIModel.Size_S)
                    sw.WriteLine("Size_S = 1");
                else
                    sw.WriteLine("Size_S = 0");
                if (m_UIManager.UIModel.Size_M)
                    sw.WriteLine("Size_M = 1");
                else
                    sw.WriteLine("Size_M = 0");
                if (m_UIManager.UIModel.Size_L)
                    sw.WriteLine("Size_L = 1");
                else
                    sw.WriteLine("Size_L = 0");
                if (m_UIManager.UIModel.Size_XL)
                    sw.WriteLine("Size_XL = 1");
                else
                    sw.WriteLine("Size_XL = 0");
                if (m_UIManager.UIModel.Size_Other)
                    sw.WriteLine("Size_Other = 1");
                else
                    sw.WriteLine("Size_Other = 0");
            }
        }

        public void LoadConfig()
        {
            string path = Environment.CurrentDirectory;

            List<string> config = new List<string>();
            if (File.Exists(path + "\\X4_Editor.cfg"))
            {
                using (StreamReader sr = new StreamReader(path + "\\X4_Editor.cfg"))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.Contains("Vanilla X4 Path: "))
                        {
                            line = line.Replace("Vanilla X4 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.Path = line;
                        }
                        else if (line.Contains("Mod 1 Path: "))
                        {
                            line = line.Replace("Mod 1 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath1 = line;
                        }
                        else if (line.Contains("Mod 2 Path: "))
                        {
                            line = line.Replace("Mod 2 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath2 = line;
                        }
                        else if (line.Contains("Mod 3 Path: "))
                        {
                            line = line.Replace("Mod 3 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath3 = line;
                        }
                        else if (line.Contains("Mod 4 Path: "))
                        {
                            line = line.Replace("Mod 4 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath4 = line;
                        }
                        else if (line.Contains("Mod 5 Path: "))
                        {
                            line = line.Replace("Mod 5 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath5 = line;
                        }
                        else if (line.Contains("Mod 6 Path: "))
                        {
                            line = line.Replace("Mod 6 Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ModPath6 = line;
                        }
                        else if (line.Contains("Mod export Path: "))
                        {
                            line = line.Replace("Mod export Path: ", "");
                            if (line.Length > 0)
                                m_UIManager.UIModel.ExportPath = line;
                        }

                        // filters
                        if (line.Contains("Ships = 1"))
                            m_UIManager.UIModel.Ships = true;
                        if (line.Contains("Ships = 0"))
                            m_UIManager.UIModel.Ships = false;
                        if (line.Contains("Shields = 1"))
                            m_UIManager.UIModel.Shields = true;
                        if (line.Contains("Shields = 0"))
                            m_UIManager.UIModel.Shields = false;
                        if (line.Contains("Engines = 1"))
                            m_UIManager.UIModel.Engines = true;
                        if (line.Contains("Engines = 0"))
                            m_UIManager.UIModel.Engines = false;
                        if (line.Contains("Weapons = 1"))
                            m_UIManager.UIModel.Weapons = true;
                        if (line.Contains("Weapons = 0"))
                            m_UIManager.UIModel.Weapons = false;
                        if (line.Contains("Wares = 1"))
                            m_UIManager.UIModel.Wares = true;
                        if (line.Contains("Wares = 0"))
                            m_UIManager.UIModel.Wares = false;
                        if (line.Contains("Size_S = 1"))
                            m_UIManager.UIModel.Size_S = true;
                        if (line.Contains("Size_S = 0"))
                            m_UIManager.UIModel.Size_S = false;
                        if (line.Contains("Size_M = 1"))
                            m_UIManager.UIModel.Size_M = true;
                        if (line.Contains("Size_M = 0"))
                            m_UIManager.UIModel.Size_M = false;
                        if (line.Contains("Size_L = 1"))
                            m_UIManager.UIModel.Size_L = true;
                        if (line.Contains("Size_L = 0"))
                            m_UIManager.UIModel.Size_L = false;
                        if (line.Contains("Size_XL = 1"))
                            m_UIManager.UIModel.Size_XL = true;
                        if (line.Contains("Size_XL = 0"))
                            m_UIManager.UIModel.Size_XL = false;
                        if (line.Contains("Size_Other = 1"))
                            m_UIManager.UIModel.Size_Other = true;
                        if (line.Contains("Size_Other = 0"))
                            m_UIManager.UIModel.Size_Other = false;
                    }
                }
            }
        }
    }
}
