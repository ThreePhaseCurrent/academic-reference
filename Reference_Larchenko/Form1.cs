using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Reference_Larchenko
{
    public partial class Form1 : Form
    {
        SQLiteConnection conn;
        Word.Application application;
        Word.Document document;
          
        public Form1()
        { 
            InitializeComponent();

            //NewWdDoc2();
            // NewDocWord();

            //WdBookMarkps();

            //ReplaceInWordExemple();

            //OpenWordDocumentExemple();
        }

        //создание документа с нуля
        private void NewWdDoc2()
        {
            application = new Word.Application();
            application.Documents.Add();

            document = application.Documents[1];

            //Вставляем в документ 4 параграфа
            object oMissing = System.Reflection.Missing.Value;
            document.Paragraphs.Add(ref oMissing);
            document.Paragraphs.Add(ref oMissing);

            Word.Range range = application.Documents[1].Paragraphs[1].Range;

            //Переходим к первому добавленному параграфу
            Word.Paragraph wordparagraph = document.Paragraphs[1];

            Word.Range wordrange = wordparagraph.Range;
            //Добавляем таблицу в начало второго параграфа
            Object defaultTableBehavior =
             Word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior =
             Word.WdAutoFitBehavior.wdAutoFitWindow;
            Word.Table wordtable1 = document.Tables.Add(wordrange, 1, 2,
              ref defaultTableBehavior, ref autoFitBehavior);
            //Сдвигаемся вниз в конец документа
            object unit;
            object extend;
            unit = Word.WdUnits.wdStory;
            extend = Word.WdMovementType.wdMove;
            application.Selection.EndKey(ref unit, ref extend);

            document.Tables[1].Rows[1].Cells[2].Range.Font.Name = @"Times New Roman";
            document.Tables[1].Rows[1].Cells[2].Range.Text = @"Державний вищий навальний заклад «Криворізький національний університет» State Institution of Higner Education" + Environment.NewLine + "«Kryvyi Rig National University»";

            //выравнивание по центру
            document.Tables[1].Rows[1].Cells[2].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //отступ после
            document.Tables[1].Rows[1].Cells[2].Range.ParagraphFormat.SpaceAfter = 0;
            //отступ до
            document.Tables[1].Rows[1].Cells[2].Range.ParagraphFormat.SpaceBefore = 0;
            //одинарный интервал
            document.Tables[1].Rows[1].Cells[2].Range.ParagraphFormat.Space1();

            document.Tables[1].Rows[1].Cells[2].Range.Font.Size = 12;
            //RGB цвет
            Color c = Color.FromArgb(23, 74, 177);
            var myWdColor = (Microsoft.Office.Interop.Word.WdColor)(c.R + 0x100 * c.G + 0x10000 * c.B);
            document.Tables[1].Rows[1].Cells[2].Range.Font.Color = myWdColor;

            Word.Range range2 = application.Selection.Range;

            range2.Text = "Академічна довідка" + Environment.NewLine;
            range2.Font.Name = @"Times New Roman";
            range2.Font.Size = 20;
            range2.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            range2.Font.Bold = 2;
            range2.ParagraphFormat.SpaceAfter = 0;
            range2.ParagraphFormat.Space1();

            Word.Range range3 = application.Selection.Range;

            unit = Word.WdUnits.wdStory;
            extend = Word.WdMovementType.wdMove;
            int pos = application.Selection.EndKey();


            ////Вставляем таблицу по месту курсора
            //Word.Table wordtable2 = document.Tables.Add(
            //  application.Selection.Range, 1, 2, 
            //    ref defaultTableBehavior, ref autoFitBehavior);

            //document.Tables[2].Borders.Enable = 1;



            //Меняем стили созданных таблиц
            //Object style = "Классическая таблица 1";
            //wordtable1.set_Style(ref style);
            //style = "Сетка таблицы 3";
            //Object applystyle = true;
            //wordtable2.set_Style(ref style);
            //wordtable2.ApplyStyleFirstColumn = true;
            //wordtable2.ApplyStyleHeadingRows = true;
            //wordtable2.ApplyStyleLastRow = false;
            //wordtable2.ApplyStyleLastColumn = false;

            application.ActiveDocument.Save();
            application.ActiveDocument.Close();
            application.Quit();
        }

        //создание нового документа ворд
        private void NewDocWord()
        {
            try
            {
                //создание документа
                application = new Word.Application();
                object missing = Type.Missing;
                application.Documents.Add();
            
                Word.Paragraph para = application.Documents[1].Paragraphs.Add();

                for (int i = 0; i < 5; i++)
                {
                    application.Documents[1].Paragraphs.Add();
                }

                Word.Range range = application.Documents[1].Paragraphs[1].Range;
                Word.Table table = application.Documents[1].Paragraphs[1].Range.Tables.Add(range, 1, 2);
                //application.Documents[1].Tables.Add(range, 1, 2);
                table.Borders.Enable = 1;

                Word.Range range2 = application.Documents[1].Paragraphs[2].Range;
                range2.InsertAfter("123");

                

                //range.Font.Name = @"Times New Roman";
                //table.Rows[1].Cells[2].Range.Text = @"Державний вищий навальний заклад «Криворізький національний університет» State Institution of Higner Education" + Environment.NewLine + "«Kryvyi Rig National University»";

                ////выравнивание по центру
                //table.Rows[1].Cells[2].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                ////отступ после
                //table.Rows[1].Cells[2].Range.ParagraphFormat.SpaceAfter = 0;
                ////отступ до
                //table.Rows[1].Cells[2].Range.ParagraphFormat.SpaceBefore = 0;
                ////одинарный интервал
                //table.Rows[1].Cells[2].Range.ParagraphFormat.Space1();

                //table.Rows[1].Cells[2].Range.Font.Size = 12;
                ////RGB цвет
                //Color c = Color.FromArgb(23, 74, 177);
                //var myWdColor = (Microsoft.Office.Interop.Word.WdColor)(c.R + 0x100 * c.G + 0x10000 * c.B);
                //table.Rows[1].Cells[2].Range.Font.Color = myWdColor;

                //object missing = Type.Missing;
                //Word.Paragraph para = document.Paragraphs.Add(ref missing);

                //for (int i = 0; i < 5; i++)
                //{
                //    document.Paragraphs.Add(ref missing);
                //}

                //document.Paragraphs[1].Range.Text = "Академічна довідка";




                //foreach(Word.Row row in table.Rows)
                //{
                //    foreach(Word.Cell cell in row.Cells)
                //    {
                //        if (cell.RowIndex == 1)
                //        {
                //            cell.Range.Text = "Колонка " + cell.ColumnIndex.ToString();
                //            cell.Range.Bold = 1;
                //            cell.Range.Font.Name = "verdana";
                //            cell.Range.Font.Size = 10;

                //            cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //        }
                //    }
                //}            
                }
                catch
                {
                    MessageBox.Show("Error!");
                }
                finally
                {
                application.ActiveDocument.Save();
                application.ActiveDocument.Close();
                application.Quit();
            }
        }

        //подключение к бд при запуске приложения
        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new SQLiteConnection("Data Source=Reference.db; Version=3");

            try
            {
                conn.Open();
            }
            catch { MessageBox.Show("Соеднение c БД не установлено!"); }
        }

        //пример заполнения шаблона с помощью закладок
        private void WdBookMarkps()
        {
            //создаем обьект приложения word
            application = new Word.Application();
            // создаем путь к файлу
            Object templatePathObj = @"12.docx";
            application.Documents.Open(templatePathObj);


            Word.Range bookmarkRange = application.Documents[1].Bookmarks.get_Item(1).Range;
            bookmarkRange.Text = @"Ларченко Антон Дмитриевич";

            bookmarkRange = application.Documents[1].Bookmarks.get_Item(2).Range;
            bookmarkRange.Text = @"Larchenko Anton Dmitrievich";

            application.ActiveDocument.Save();
            application.ActiveDocument.Close();
            application.Quit();
        }

        //пример заполнения ячеек таблицы
        private void FillingTable()
        {
            try
            {
                //создаем обьект приложения word
                application = new Word.Application();
                // создаем путь к файлу
                Object templatePathObj = @"E:\1.docx";
                //document = application.Documents.Add(ref templatePathObj, ref missingObj, ref missingObj, ref missingObj);
                application.Documents.Open(templatePathObj);

                Word.Table tables = application.Documents[1].Tables[3];

                tables.Rows[1].Cells[2].Range.Text = "Larchenko";
                tables.Rows[1].Cells[2].Range.InlineShapes.AddHorizontalLineStandard(tables.Rows[1].Cells[2].Range);
            }
            catch { MessageBox.Show("Error!"); }
            finally
            {

                application.ActiveDocument.Save();
                application.ActiveDocument.Close();
                application.Quit();
            }
        }

        //обычное открытие документа
        private void OpenWordDocumentExemple()
        {
            //создаем обьект приложения word
            application = new Word.Application();
            // создаем путь к файлу
            Object templatePathObj = @"E:\1.docx";
            Object trueObj = true;
            Object falseObj = false;
            Object missingObj = System.Reflection.Missing.Value;

            // если вылетим не этом этапе, приложение останется открытым
            try
            {
                document = application.Documents.Add(ref templatePathObj, ref missingObj, ref missingObj, ref missingObj);
            }
            catch (Exception error)
            {
                document.Close(ref falseObj, ref missingObj, ref missingObj);
                application.Quit(ref missingObj, ref missingObj, ref missingObj);
                document = null;
                application = null;
                throw error;
            }
            application.Visible = true;
        }

        private void CreateRef_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            CreateReference createReference = new CreateReference(ref conn);
            createReference.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
