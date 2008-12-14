using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using WebChart;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.DirectoryServices;

namespace KSMD_Labs
{
    public partial class MainForm : Form
    {
        //для лога
        ArrayList Log;
        //диалог открытия
        OpenFileDialog opnFileDlg;
        // Массив отсчётов ЭКГ
        List<int> y;
        // Массивы маски Т в секундах
        List<double> T;
        // .. в количестве отсчётов
        List<double> Td;
        // Массив порогов
        List<int> G;
        // Массив длительности RR- интрервалов
        List<int> RR;
        ////////////////////////////       
        int int_n, // Начало диапозона поиска
            int_k; // Конец диапазона поиска
        // Массивы индексов
        List<int> indP,
                  indQ,
                  indR,
                  indS,
                  indT;
        ////////////////////////////
        //LAB 3
        ////////////////////////////
        // Новый, "уравненный" сигнал
        List<int> newY, XA0, YA0;
        // Границы, вершина и амплитуда соответствующих зубцов
        ArrayList peakP, peakQ, peakR, peakS, peakT;

        ////////////////////////////
        //LAB 4
        ////////////////////////////
        // Зашумленный сигнал
        List<int> NoiseY;
        // Линейно отфильрованный сигнал
        List<int> LinearY;
        // Скачкообразно отфильтрованный сигнал
        List<int> SpasmodicY;
        // Размах сигнала
        int AMs;
        // Коэффициент сигнал/шум по индивидуальному заданию
        const double KoefSigNoise = 0.5;
        /*!!!!!!!!
         * Границы комплекса QRS расчитаны по закону:
         * вершина R +- epsQRSbounds отсчетов
         */
        const int epsQRSbounds = 10;
        // частота шума, Гц
        const int noiseFreq = 1000; //Hz
        // размах шума
        double AMnoise;
        // размер апертуры фильтра
        const int aperture_size = 3;
        // чисто шум до фильтраций
        List<int> PureNoiseY;
        // чисто шум после линейной фильтрации
        List<int> PureNoiseYLinear;
        // чисто шум после скачкообразной фильтрации
        List<int> PureNoiseYSpasmodic;
        // коэффициенты фильтраций
        double coefLinear;
        double coefSpasmodic;
        ////////////////////////////



        public MainForm()   // Конструктор
        {
            InitializeComponent();
            Log = new ArrayList();
            opnFileDlg = new OpenFileDialog();
            //
            // Выделяем память для остальных обьектов
            //
            this.y = new List<int>();
            this.T = new List<double>();
            this.Td = new List<double>();
            this.G = new List<int>();
            this.RR = new List<int>();

            this.indP = new List<int>();
            this.indQ = new List<int>();
            this.indR = new List<int>();
            this.indS = new List<int>();
            this.indT = new List<int>();

            newY = new List<int>();
            XA0 = new List<int>();
            YA0 = new List<int>();
            peakP = new ArrayList();
            peakQ = new ArrayList();
            peakR = new ArrayList();
            peakS = new ArrayList();
            peakT = new ArrayList();
            // Добавляем элементы временно маски в секундах
            T.Add(1);
            T.Add(0.16);
            T.Add(0.035);
            T.Add(0.5);
            T.Add(0.75);                           
            //////////////////////////////////////////////
            // Пороговые значения в единицах шкалы АЦП
            G.Add(15);
            G.Add(1);
            G.Add(1);
            G.Add(2);
            G.Add(1);
            ////////////////////////////////////////////
            NoiseY = new List<int>();
            LinearY = new List<int>();
            SpasmodicY = new List<int>();
            PureNoiseY = new List<int>();
            PureNoiseYLinear = new List<int>();
            PureNoiseYSpasmodic = new List<int>();
            ////////////////////////////////////////////
            btnLab2.Enabled=false;
            btnLab3.Enabled=false;
            btnLab4.Enabled = false;
        }   // Конец КОНСТРУКТОРА

        private void openFileButton_Click(object sender, EventArgs e)
        {
            // Открываем исследуемый файл
            opnFileDlg.Filter = "Файл данных ЭКГ(*.dat)|*.dat";
            opnFileDlg.RestoreDirectory = true;
            if (opnFileDlg.ShowDialog() == DialogResult.OK)
            {
                btnLab2.Enabled=true;
                btnLab3.Enabled=true;
                btnLab4.Enabled = true;
            }
        }
        void WriteLog2File()
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText("C:\\dron_results.log");
                for (int i = 0; i < Log.Count; i++)
                    writer.Write(Log[i]);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        ///  Функция поиска зубца R
        /// </summary>
        /// <param name="int_n"></param>
        /// Индекс начала зоны поиска
        /// <param name="int_k"></param>
        /// Индекс конца зоны поиска
        /// <returns>
        /// Индекс текущего зубца R
        /// </returns>
        private int Find_R(int int_n, int int_k)
        {
            Log.Add("Finding R: "+int_n.ToString()+"-"+int_k.ToString()+". ");
            if ((int_n < 0) || (int_k < 0))
                return 0;
            if (int_n == 0)
                int_n = 1;
            if (int_k > y.Count - 1)
                int_k = y.Count - 1;
            if (int_n > y.Count - 1)
                int_n = y.Count - 1;
            int m_y = y[int_n],
                n_max = int_n;
            // Поиск экстремума в выделенной зоне
            for (int i = int_n; i <= int_k; i++)
                if (y[i] > m_y)
                {
                    m_y = y[i];
                    n_max = i;
                }
            // Проверка принадлежности зубца по пороговому значанию
            if (!((y[n_max] - y[n_max - 1]) >= G[0]))
            //    n_max=n_max;
            //else
                n_max=0;
            Log[Log.Count - 1] += ("Ret: "+n_max.ToString()+"\n");
            return n_max;
        }
        /// <summary>
        ///  Функция поиска зубца P или T
        /// </summary>
        /// <param name="int_n"></param>
        /// Индекс начала зоны поиска
        /// <param name="int_k"></param>
        /// Индекс конца зоны поиска
        /// <param name="nom"></param>
        /// Номер зубца (2 - P, 5 - T)
        /// <returns>
        /// Индекс текущего зубца P T
        /// </returns>
        private int Find_P_T(int int_n, int int_k, int nom)
        {
            Log.Add("Finding ");
            if (nom==2)
                Log[Log.Count-1]+="P";
            else if (nom==5)
                Log[Log.Count-1]+="T";
            else
                Log[Log.Count - 1] += "?";
            Log[Log.Count-1]+=(": " + int_n.ToString() + "-" + int_k.ToString() + ". ");
            if ((int_n < 0) || (int_k < 0))
                return 0;
            if (int_n == 0)
                int_n = 1;
            if (int_k > y.Count - 1)
                int_k = y.Count - 1;
            if (int_n > y.Count - 1)
                int_n = y.Count - 1;
            int m_y = y[int_n],
                n_max = int_n;
            // Поиск экстремума в выделенной зоне
            for (int i = int_n; i <= int_k; i++)
                if (y[i] > m_y)
                {
                    m_y = y[i];
                    n_max = i;
                }
            // Проверка принадлежности зубца по пороговому значанию
            if (!(((y[n_max] - y[n_max - 1]) >= G[nom - 1]) &&
                ((y[n_max] - y[n_max - 1]) < G[0])))
                //    return n_max;
                //else
                n_max = 0;
            Log[Log.Count - 1] += ("Ret: " + n_max.ToString() + "\n");
            return n_max;

        }
        /// <summary>
        ///  Функция поиска зубца Q и S
        /// </summary>
        /// <param name="int_n"></param>
        ///  Индекс начала зоны поиска
        /// <param name="int_k"></param>
        ///  Индекс конца зоны поиска
        /// <param name="nom"></param>
        ///  Номер зубца (3 - Q, 4 - S)
        /// <returns> 
        ///  Индекс текущего зубца Q или S
        /// </returns>
        private int Find_Q_S(int int_n, int int_k, int nom)
        {
            Log.Add("Finding ");
            if (nom == 3)
                Log[Log.Count - 1] += "Q";
            else if (nom == 4)
                Log[Log.Count - 1] += "S";
            else
                Log[Log.Count - 1] += "??";
            Log[Log.Count - 1] += (": " + int_n.ToString() + "-" + int_k.ToString() + ". ");
            if ((int_n < 0) || (int_k < 0))
                return 0;
            if (int_n == 0)
                int_n = 1;
            if (int_k > y.Count - 1)
                int_k = y.Count - 1;
            if (int_n > y.Count - 1)
                int_n = y.Count - 1;
            int m_y = y[int_n],
                n_min = int_n;
            // Поиск экстремума в выделенной зоне
            for (int i = int_n; i <= int_k; i++)
                if (y[i] < m_y)
                {
                    m_y = y[i];
                    n_min = i;
                }
            // Проверка принадлежности зубца по пороговому значанию
            if (!(Math.Abs(y[n_min] - y[n_min - 1]) >= G[nom - 1]) )
            //    return n_min;
            //else
                n_min=0;
            Log[Log.Count - 1] += ("Ret: " + n_min.ToString() + "\n");
            return n_min;
        }
        /// <summary>
        /// Функция формирования массмва индексов зубцов P и T.
        /// </summary>
        /// <param name="nom"></param>
        /// Номер зубца (2 - P, 5 - T)
        /// <param name="i"></param>
        /// Текущая итерация
        /// <returns>
        /// Индекс зубца P или T
        /// </returns>
        private int Display_P_T(int nom, int i)
        {
            // Переменная для возврата
            int retValue = 0;
            int_n = (int)(Math.Round(indR[i + 1] - 1.1 * Td[nom - 1]));
            int_k = (int)(Math.Round(indR[i + 1] - 0.75 * Td[nom - 1]));
            retValue = Find_P_T(int_n, int_k, nom);
            if (retValue == 0)
            {
                int_k = (int)(Math.Round(int_k + 0.5 * Td[nom - 1]));
                int_n = (int)(Math.Round(int_n - 0.5 * Td[nom - 1]));
                retValue = Find_P_T(int_n, int_k, nom);
            }
            return retValue;
        }
        /// <summary>
        /// Процедура формирования массива индексов зубцов Q и S
        /// </summary>
        /// <param name="nom"></param>
        /// Номер зубца (3 - Q, 4 - S)
        /// <param name="i"></param>
        /// <returns>
        /// Индекс зубца
        /// </returns>
        private int Display_Q_S(int nom, int i)
        {
            int retValue = 0;
            if (nom == 3)
            {
                int_n = (int)(Math.Round(indR[i + 1] - 1.5 * Td[nom - 1]));
                int_k = (int)(Math.Round(indR[i + 1] - 0.5 * Td[nom - 1]));
            }
            if (nom == 4)
            {
                int_n = (int)(Math.Round(indR[i + 1] + 2/3 * Td[nom - 1]));
                int_k = (int)(Math.Round(indR[i + 1] + 4/3 * Td[nom - 1]));
            }
            retValue = Find_Q_S(int_n, int_k, nom);
            if (retValue == 0)
            {
                int_k = (int)(Math.Round(int_k + 0.5 * Td[nom - 1]));
                int_n = (int)(Math.Round(int_n - 0.5 * Td[nom - 1]));
                retValue = Find_Q_S(int_n, int_k, nom);
            }
            return retValue;
        }
        private void btnLab2_Click(object sender, EventArgs e)
        {
            try
            {
                // Чистим переменные перед повторным запуском
                y.Clear();
                RR.Clear();
                indP.Clear();
                indQ.Clear();
                indR.Clear();
                indS.Clear();
                indT.Clear();
                Td.Clear();
                // Пересчёт временной маски из секунд в количество отсчётов
                for (int i = 0; i < 5; i++)
                    Td.Add(T[i] * int.Parse(textBox1.Text));
                // отображаем имя файла
                label3.Text = opnFileDlg.SafeFileName;
                // очищаем заполненную таблицу зубцов
                dataGridView.Rows.Clear();
                // Создаём поток для работы с файлом
                StreamReader fileStream = new StreamReader(opnFileDlg.FileName);
                // Открываем файл в поток                
                using (fileStream)
                {

                    string inFile = fileStream.ReadToEnd();// читаем содержимое построчно

                    // Разделяем файл по пробелам
                    string[] SplFirst = inFile.Split('\r', '\n');
                    foreach (string intString in SplFirst)
                    {
                        if (intString != "")
                            y.Add(int.Parse(intString));    // Парсим
                    }
                }
                //DrawGraphic(pictureBox1,y,"Исходная ЭКГ");
                Log.Add("Count: " + y.Count.ToString() + "\n");
                // Выставим диапазон начала и конца поиска
                int_n = 0;
                int_k = (int)(Math.Round(Td[0] + 0.1 * Td[0]) - 1);
                // Поиск первого зубца R
                indR.Add(Find_R(int_n, int_k));
                for (int i = 1; i < 6; i++)
                {
                    // Определим границы зоны поиска текущего зубца
                    int_n = (int)Math.Round(indR[i - 1] + 0.9 * Td[0]);
                    int_k = (int)Math.Round(indR[i - 1] + 1.1 * Td[0]);
                    // Поиск текущего зубца R
                    indR.Add(Find_R(int_n, int_k));
                    if (indR[i] == 0)
                    {
                        int_k = (int)(Math.Round(int_k + Td[0] * 0.2));
                        int_n = (int)(Math.Round(int_n - Td[0] * 0.2));
                        indR[i] = Find_R(int_n, int_k);
                    }
                }
                // Корректировка временной маски
                for (int i = 0; i < 5; i++)
                {
                    RR.Add(indR[i + 1] - indR[i]);
                    if ((RR[i] < (Td[0] - Td[0] / 20)) ||
                        (RR[i] > (Td[0] + Td[0] / 20)))
                        for (int j = 0; j < 5; j++)
                            Td[j] = Td[j] * RR[i] / Td[0];
                    // Формирование массивов индексов зубцов P,T,Q,S
                    indP.Add(Display_P_T(2, i));
                    indT.Add(Display_P_T(5, i));
                    indQ.Add(Display_Q_S(3, i));
                    indS.Add(Display_Q_S(4, i));
                    //
                }
                // Добавляем строки в просмотрщик таблицы
                this.dataGridView.DataSource = null;
                DataGridViewRow[] newRow = new DataGridViewRow[5];
                for (int i = 0; i < 5; i++)
                {
                    // Формируем строку
                    newRow[i] = new DataGridViewRow();
                    DataGridViewTextBoxCell pNumText = new DataGridViewTextBoxCell();
                    pNumText.Value = i + 1;
                    newRow[i].Cells.Add(pNumText);
                    DataGridViewTextBoxCell pText = new DataGridViewTextBoxCell();
                    pText.Value = indP[i];
                    newRow[i].Cells.Add(pText);
                    DataGridViewTextBoxCell qText = new DataGridViewTextBoxCell();
                    qText.Value = indQ[i];
                    newRow[i].Cells.Add(qText);

                    DataGridViewTextBoxCell rText = new DataGridViewTextBoxCell();
                    rText.Value = indR[i + 1];
                    newRow[i].Cells.Add(rText);

                    DataGridViewTextBoxCell sText = new DataGridViewTextBoxCell();
                    sText.Value = indS[i];
                    newRow[i].Cells.Add(sText);
                    DataGridViewTextBoxCell tText = new DataGridViewTextBoxCell();
                    tText.Value = indT[i];
                    newRow[i].Cells.Add(tText);
                    // Добавляем строку
                    this.dataGridView.Rows.Add(newRow[i]);
                }
                //WriteLog2File();
            }
            catch (Exception ex)
            {
                // Если ошибка, то вывести текст ошибки
                MessageBox.Show(ex.Message);
            }
        }
        private int Sign(int k)
        {
            if (k < 0)
                return (-1);
            else if (k > 0)
                return (+1);
            else
                return 0;
        }
        private int FindBPeak(int r_b,int h_zub,int kol)
        {
            int b_zub=0;  //return
            int d_max = 0, d_sr;
            for (int l = -1; l >= (-r_b); l--)
            {
                int res = Math.Abs(newY[h_zub + l] - newY[h_zub + l - 1]);
                if (res > d_max)
                    d_max = res;
            }
            for (int l = -1; l >= (-r_b); l--)
            {
                d_sr = 0;
                for (int m = 0; m < kol; m++)
                    d_sr = d_sr + Math.Abs(newY[h_zub + l - m] - newY[h_zub + l - m + 1]);
                //d_sr = d_sr / kol;
                if (((Sign(newY[h_zub + l]) != Sign(newY[h_zub + l + 1])) || ((float)(d_sr / kol) <= (float)(0.2 * d_max))) && (b_zub == 0))
                    b_zub = h_zub + l + 1;
            }
            return b_zub;
        }
        private int FindEPeak(int r_e, int h_zub, int kol)
        {
            int e_zub = 0;  //return
            int d_max = 0, d_sr;
            for (int l = 1; l <= r_e; l++)
            {
                int res = Math.Abs(newY[h_zub + l] - newY[h_zub + l - 1]);
                if (res > d_max)
                    d_max = res;
            }
            for (int l = 1; l <= r_e; l++)
            {
                d_sr = 0;
                for (int m = 0; m < kol; m++)
                    d_sr = d_sr + Math.Abs(newY[h_zub + l + m] - newY[h_zub + l + m - 1]);
                //d_sr = d_sr / kol;
                if (((Sign(newY[h_zub + l]) != Sign(newY[h_zub + l - 1])) || ((float)(d_sr / kol) <= (float)(0.2 * d_max))) && (e_zub == 0))
                    e_zub = h_zub + l - 1;
            }
            return e_zub;
        }
        private void btnLab3_Click(object sender, EventArgs e)
        {
            //тут выполняем задание лабы №3
            if (y.Count == 0)
                return;
            //подготовка массивов к коррекции сигнала
            XA0.Clear();
            YA0.Clear();
            newY.Clear();
            peakP.Clear();
            peakQ.Clear();
            peakR.Clear();
            peakS.Clear();
            peakT.Clear();
            GridLimsP.Rows.Clear();
            GridLimsQ.Rows.Clear();
            GridLimsR.Rows.Clear();
            GridLimsS.Rows.Clear();
            GridLimsT.Rows.Clear();
            //нахождение индексов
            XA0.Add(0);
            for (int i = 0; i < indT.Count; i++)
                if ((indP[i] != 0) && (indT[i] != 0))
                    XA0.Add((indT[i] + indP[i]) / 2);
            XA0.Add(y.Count - 1);
            //определение амплитуд индексов
            YA0.Add(y[XA0[0]]);
            for (int i = 1; i < XA0.Count - 1; i++)   //пропускаем крайние точки
            {
                int sum = 0;
                for (int j = -10; j <= 10; j++)
                    sum += y[XA0[i] + j];
                YA0.Add((int)Math.Round(sum / 21.0));
            }
            YA0.Add(y[XA0[XA0.Count - 1]]);
            //корректировка сигнала в новый массив
            int ii = 0;
            for (int j = 0; j < y.Count; j++)
            {
                if (!((XA0[ii] <= j) && (j<=XA0[ii + 1])))
                    ii++;
                double tan = (double)(YA0[ii + 1] - YA0[ii]) / (XA0[ii + 1] - XA0[ii]);
                newY.Add((int)Math.Round(y[j]- (YA0[ii]+tan*(j-XA0[ii]))  ));
            }
            DrawGraphic(pictureBox2,newY,"Скорректированная ЭКГ");
            ////////////////////////////////////////////////
            //поиск Q
            int LimB=0,LimE=0;
            for (int i=0;i<indQ.Count;i++)      //поиск подходящего расстояния между Q и R
            {
                if ((indQ[i] != 0) && (indR[i + 1] != 0))
                {
                    LimB = (indR[i + 1] - indQ[i]);     //нашли расстояние
                    break;
                }
                if (i == (indQ.Count - 1))  //не нашли расстояния, задаем по умолчанию
                {
                    LimB = 20;
                }
            }
            //LimB = 20;          //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            LimE = LimB;    //для Q равны ?
            FindLimits(indQ,peakQ,LimB,LimE);
            // Добавляем строки в просмотрщик таблицы
            ToGridLimits(peakQ, GridLimsQ);
            ////////////////
            //поиск S
            for (int i = 0; i < indS.Count; i++)      //поиск подходящего расстояния между S и R
            {
                if ((indS[i] != 0) && (indR[i + 1] != 0))
                {
                    LimB = (indS[i] - indR[i+1]);     //нашли расстояние
                    break;
                }
                if (i == (indS.Count - 1))  //не нашли расстояния, задаем по умолчанию
                {
                    LimB = 12;
                }
            }
            LimE = LimB;    //для S равны ?
            FindLimits(indS, peakS, LimB, LimE);
            // Добавляем строки в просмотрщик таблицы
            ToGridLimits(peakS, GridLimsS);
            ////////////////
            //поиск P
            for (int i = 0; i < indP.Count; i++)      //поиск подходящего расстояния между Q и P
            {
                if ((indQ[i] != 0) && (indP[i] != 0))
                {
                    LimB = (indQ[i] - indP[i]);     //нашли расстояние
                    break;
                }
                if (i == (indP.Count - 1))  //не нашли расстояния, задаем по умолчанию
                {
                    LimB = 25;
                }
            }
            LimE = LimB;    //для P равны
            FindLimits(indP, peakP, LimB, LimE);
            // Добавляем строки в просмотрщик таблицы
            ToGridLimits(peakP, GridLimsP);
            ////////////////
            //поиск T
            for (int i = 0; i < indT.Count; i++)      //поиск подходящего расстояния между S и T
            {
                if ((indS[i] != 0) && (indT[i] != 0))
                {
                    LimB = (indT[i] - indS[i]);     //нашли расстояние
                    break;
                }
                if (i == (indT.Count - 1))  //не нашли расстояния, задаем по умолчанию
                {
                    LimB = 25;
                }
            }
            LimE = LimB;    //для T равны
            FindLimits(indT, peakT, LimB, LimE);
            // Добавляем строки в просмотрщик таблицы
            ToGridLimits(peakT, GridLimsT);
            ////////////////
            //поиск R
            LimE = LimB=15;
            FindLimits(indR, peakR, LimB, LimE);
            // Добавляем строки в просмотрщик таблицы
            ToGridLimits(peakR, GridLimsR);            
        }
        private void FindLimits(List<int> aPeaks,ArrayList aLimPeaks, int aLimB,int aLimE)
        {
            for (int i = 0; i < aPeaks.Count; i++)
            {
                int[] ind_bey = new int[4];
                if (aPeaks[i] == 0)
                {
                    for (int j = 0; j < 4; j++)
                        ind_bey[j] = 0;
                }
                else
                {
                    ind_bey[0] = FindBPeak(aLimB, aPeaks[i], 15);
                    ind_bey[1] = aPeaks[i];
                    ind_bey[2] = FindEPeak(aLimE, aPeaks[i], 15);
                    ind_bey[3] = newY[aPeaks[i]];
                }
                aLimPeaks.Add(ind_bey);
            }
        }
        private void ToGridLimits(ArrayList aLimPeaks,DataGridView agrid)
        {
            agrid.DataSource = null;
            DataGridViewRow[] newRow = new DataGridViewRow[aLimPeaks.Count];
            for (int i = 0; i < aLimPeaks.Count; i++)
            {
                // Формируем строку
                newRow[i] = new DataGridViewRow();
                DataGridViewTextBoxCell numText = new DataGridViewTextBoxCell();
                numText.Value = i + 1;
                newRow[i].Cells.Add(numText);
                DataGridViewTextBoxCell bText = new DataGridViewTextBoxCell();
                bText.Value = ((int[])aLimPeaks[i])[0];
                newRow[i].Cells.Add(bText);
                DataGridViewTextBoxCell mText = new DataGridViewTextBoxCell();
                mText.Value = ((int[])aLimPeaks[i])[1];
                newRow[i].Cells.Add(mText);
                DataGridViewTextBoxCell eText = new DataGridViewTextBoxCell();
                eText.Value = ((int[])aLimPeaks[i])[2];
                newRow[i].Cells.Add(eText);
                DataGridViewTextBoxCell yText = new DataGridViewTextBoxCell();
                double yy = ((int[])aLimPeaks[i])[3] / 400.0;
                yText.Value = yy.ToString();
                newRow[i].Cells.Add(yText);

                // Добавляем строку
                agrid.Rows.Add(newRow[i]);
            }
        }
        private void DrawGraphic(PictureBox aImage, List<int> aData, String aTitle)
        {
            /////////////////////////////////////////////
            //вывод в новый график
            //--------------------------------------
            // Создаём и рисуем график
            ChartEngine engine = new ChartEngine();
            engine.Size = aImage.Size;
            ChartCollection charts = new ChartCollection(engine);
            engine.Charts = charts;
            ///////////////////////////
            engine.TopPadding = 20;
            engine.Padding = 10;
            engine.ChartPadding = 20;
            engine.ShowXValues = true;
            engine.ShowYValues = true;
            engine.ShowTitlesOnBackground = false;
            /////////////////////////////////////////
            // Устанавливаем название
            ChartText title = new ChartText();
            engine.Title = title;
            Font font = new Font("Arial", 12, FontStyle.Bold);
            title.Font = font;
            title.StringFormat.Alignment = StringAlignment.Center;
            title.ForeColor = Color.Black;
            title.Text = aTitle;
            // Устанавливаем задний фон
            ChartInterior interior = engine.Background;
            interior.Type = InteriorType.LinearGradient;
            interior.Color = Color.Tan;
            interior.ForeColor = Color.FloralWhite;
            interior.StartPoint = new Point(0, 0);
            interior.EndPoint = new Point(engine.Size.Width, engine.Size.Height);
            // Установим фон самого графика
            interior = engine.PlotBackground;
            interior.Type = InteriorType.LinearGradient;
            interior.Color = Color.CornflowerBlue;
            interior.ForeColor = Color.Beige;
            interior.StartPoint = new Point(0, 0);
            interior.EndPoint = new Point(engine.Size.Width, engine.Size.Height);
            interior.Angle = 90;
            ///////////////////////////
            //создадим элементы
            ChartPointCollection data = new ChartPointCollection();
            Chart line = new LineChart(data, Color.Black);
            line.ShowLineMarkers = false;
            for (int pointCount = 0; pointCount < aData.Count; pointCount++)
            {
                data.Add(new ChartPoint(" ", aData[pointCount]));
            }
            charts.Add(line);
            engine.GridLines = GridLines.Horizontal;
            Image image = engine.GetBitmap();
            // Отобразим диаграмму на форме
            aImage.Image = image;
        }
        private void FindMinMax(List<int> aMas, int aBeg, int aEnd, out int aMax, out int aMin)
        {
            if ((aMas.Count == 0) || (aBeg >= aMas.Count) || aEnd >= aMas.Count || aBeg < 0 || aEnd < 0)
            {
                aMax = aMin = 0;
                return;
            }
            aMax = aMin = aMas[aBeg];
            for (int i = aBeg; i <= aEnd; i++)
            {
                if (aMax < aMas[i])
                    aMax = aMas[i];
                if (aMin > aMas[i])
                    aMin = aMas[i];
            }
        }

        private double Expectation(List<int> aSignal)       // математическое ожидание
        {
            double expectation = 0;
            for (int i = 0; i < aSignal.Count; i++)
                expectation += aSignal[i];
            expectation /= aSignal.Count;
            return expectation;
        }
        private double Dispersion(List<int> aSignal, double aExpectation)  // дисперсия
        {
            double dispersion = 0;
            for (int i = 0; i < aSignal.Count; i++)
                dispersion += ((aSignal[i] - aExpectation) * (aSignal[i] - aExpectation));
            dispersion /= aSignal.Count - 1;
            return dispersion;
        }
        private void btnLab4_Click(object sender, EventArgs e)
        {
            AMs = 0;    // размах сигнала
            AMnoise = 0;    //размах шума
            NoiseY.Clear();
            LinearY.Clear();
            SpasmodicY.Clear();
            PureNoiseY.Clear();
            PureNoiseYLinear.Clear();
            PureNoiseYSpasmodic.Clear();
            ////////////////////
            // определяем размах сигнала
            int ymax = newY[0],
                ymin = newY[0];
            int[] peakPrev, peakNext;
            peakPrev = (int[])peakR[0];
            // поиск в начале, до первого зубца R
            for (int i = 1; i < peakPrev[1] - epsQRSbounds; i++)
            {
                if (ymax < newY[i])
                    ymax = newY[i];
                if (ymin > newY[i])
                    ymin = newY[i];
            }
            peakPrev = (int[])peakR[peakR.Count - 1];
            // поиск в конце, после последнего зубца R
            for (int i = peakPrev[1] + epsQRSbounds; i < newY.Count; i++)
            {
                if (ymax < newY[i])
                    ymax = newY[i];
                if (ymin > newY[i])
                    ymin = newY[i];
            }
            // поиск в промежутках
            for (int i = 0; i < peakR.Count - 1; i++)     // зубцы
            {
                peakPrev = (int[])peakR[i];
                peakNext = (int[])peakR[i + 1];
                for (int k = peakPrev[1]+epsQRSbounds; k < peakNext[1]-epsQRSbounds; k++)
                {
                    if (ymax < newY[k])
                        ymax = newY[k];

                    if (ymin > newY[k])
                        ymin = newY[k];
                }
            }
            // вычисляем размах
            AMs = ymax - ymin;
            ////////////////////
            //генерирование шума
            // e(x) = AMnoise * sin( Omega * x )
            // AMnoise = KoefSigNoise * AMs * 0.5
            // Omega = 2*pi*noiseFreq / freq  ====>
            // Omega = Math.Sqrt(Math.PI) * noiseFreq / ( 60 * freq )

            // вычисляем размах (амплитуду) шума
            AMnoise = KoefSigNoise * AMs * 0.5;
            // вычисляем омегу
            double Omega = Math.PI * Math.PI * noiseFreq / ( 60 * int.Parse(textBox1.Text) );
            // копируем весь сигнал в новый сигнал
            foreach (int item in newY)
                NoiseY.Add(item);
            // генерируем шум, исключая комплексы QRS
            peakPrev = (int[])peakR[0];
            // генерируем шум в начале, до первого зубца R
            for (int i = 0; i < peakPrev[1] - epsQRSbounds; i++)
            {
                NoiseY[i] = (int)(newY[i] + (AMnoise * Math.Sin(Omega * i)) + 0.5);
            }
            peakPrev = (int[])peakR[peakR.Count - 1];
            // генерируем шум в конце, после последнего зубца R
            for (int i = peakPrev[1] + epsQRSbounds; i < newY.Count; i++)
            {
                NoiseY[i] = (int)(newY[i] + (AMnoise * Math.Sin(Omega * i)) + 0.5);
            }
            // генерируем шум в промежутках
            for (int i = 0; i < peakR.Count - 1; i++)     // зубцы
            {
                peakPrev = (int[])peakR[i];
                peakNext = (int[])peakR[i + 1];
                for (int k = peakPrev[1] + epsQRSbounds; k < peakNext[1] - epsQRSbounds; k++)
                {
                    NoiseY[k] = (int)(newY[k] + (AMnoise * Math.Sin(Omega * k)) + 0.5);
                }
            }
            //выведем график
            DrawGraphic(pictureBoxNoise, NoiseY, "Зашумленный сигнал");
            // линейная фильтрация
            // копируем весь зашумленный сигнал в новый сигнал
            foreach (int item in NoiseY)
                LinearY.Add(item);
            // пропускаем крайние точки....

            // фильтруем шум, исключая комплексы QRS
            int delimiter = 2 * aperture_size + 1;
            peakPrev = (int[])peakR[0];
            // фильтруем шум в начале, до первого зубца R, с учетом размера апертуры
            for (int i = aperture_size; i < peakPrev[1] - epsQRSbounds; i++)
            {
                int sum = 0;
                for (int j = -aperture_size; j <= aperture_size; j++)
                    sum += NoiseY[i + j];
                LinearY[i] = (int)(sum * 1.0 / delimiter + 0.5);
            }
            peakPrev = (int[])peakR[peakR.Count - 1];
            // фильтруем шум в конце, после последнего зубца R, с учетом размера апертуры
            for (int i = peakPrev[1] + epsQRSbounds; i < newY.Count - aperture_size; i++)
            {
                int sum = 0;
                for (int j = -aperture_size; j <= aperture_size; j++)
                    sum += NoiseY[i + j];
                LinearY[i] = (int)(sum * 1.0 / delimiter + 0.5);
            }
            // фильтруем шум в промежутках
            for (int i = 0; i < peakR.Count - 1; i++)     // зубцы
            {
                peakPrev = (int[])peakR[i];
                peakNext = (int[])peakR[i + 1];
                for (int k = peakPrev[1] + epsQRSbounds; k < peakNext[1] - epsQRSbounds; k++)
                {
                    int sum = 0;
                    for (int j = -aperture_size; j <= aperture_size; j++)
                        sum += NoiseY[k + j];
                    LinearY[k] = (int)(sum * 1.0 / delimiter + 0.5);
                }
            }
            DrawGraphic(pictureBoxLinearFilter, LinearY, "Линейно отфильтрованный сигнал");

            // скачкообразный фильтр
            // копируем весь зашумленный сигнал в новый сигнал
            foreach (int item in NoiseY)
                SpasmodicY.Add(item);
            // пропускаем крайние точки....

            // фильтруем шум, исключая комплексы QRS
            peakPrev = (int[])peakR[0];
            // фильтруем шум в начале, до первого зубца R, с учетом размера апертуры
            for (int i = aperture_size; i < peakPrev[1] - epsQRSbounds; i++)
            {
                int localMax, localMin;
                FindMinMax(SpasmodicY, i - aperture_size, i + aperture_size, out localMax, out localMin);
                SpasmodicY[i] = (localMin + localMax) / 2;
            }
            peakPrev = (int[])peakR[peakR.Count - 1];
            // фильтруем шум в конце, после последнего зубца R, с учетом размера апертуры
            for (int i = peakPrev[1] + epsQRSbounds; i < newY.Count - aperture_size; i++)
            {
                int localMax, localMin;
                FindMinMax(SpasmodicY, i - aperture_size, i + aperture_size, out localMax, out localMin);
                SpasmodicY[i] = (localMin + localMax) / 2;
            }
            // фильтруем шум в промежутках
            for (int i = 0; i < peakR.Count - 1; i++)     // зубцы
            {
                peakPrev = (int[])peakR[i];
                peakNext = (int[])peakR[i + 1];
                for (int k = peakPrev[1] + epsQRSbounds; k < peakNext[1] - epsQRSbounds; k++)
                {
                    int localMax, localMin;
                    FindMinMax(SpasmodicY, k - aperture_size, k + aperture_size, out localMax, out localMin);
                    SpasmodicY[k] = (localMin + localMax) / 2;
                }
            }
            DrawGraphic(pictureBoxSpasmodic, SpasmodicY, "Скачкообразно отфильтрованный сигнал");
            // вычисляем шум до и после фильтраций
            for (int i = 0; i < newY.Count; i++)
            {
                PureNoiseY.Add(NoiseY[i]-newY[i]);
                PureNoiseYLinear.Add(LinearY[i] - newY[i]);
                PureNoiseYSpasmodic.Add(SpasmodicY[i] - newY[i]);
            }
            double expBefore = Expectation(PureNoiseY);
            double dispBefore = Dispersion(PureNoiseY, expBefore);

            double expAfter = Expectation(PureNoiseYLinear);
            double dispAfter = Dispersion(PureNoiseYLinear,expAfter);
            coefLinear = 1 - (Math.Sqrt(dispAfter) / Math.Sqrt(dispBefore));

            expAfter = Expectation(PureNoiseYSpasmodic);
            dispAfter = Dispersion(PureNoiseYSpasmodic, expAfter);
            coefSpasmodic = 1 - (Math.Sqrt(dispAfter) / Math.Sqrt(dispBefore));

            textCoefLinear.Text = coefLinear.ToString();
            textCoefSpasmodic.Text = coefSpasmodic.ToString();
        }
    }
}
