using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.MicroOrm;
using Apskaita5.DAL.MySql;
using DeveloperUtils.TestClasses;

namespace DeveloperUtils
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async void executeInsertButton_ClickAsync(object sender, EventArgs e)
        {

            executeInsertButton.Enabled = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await Task.Run(async () => await InsertSimpleDocuments(new Progress<int>(value => { progressBar1.Value = value; })));
            }
            catch (Exception ex)
            {
                watch.Stop();
                insertTimeLabel.Text = string.Format("Executed in {0} ms", watch.ElapsedMilliseconds.ToString());
                executeInsertButton.Enabled = true;
                MessageBox.Show(ex.Message);
                return;
            }
            watch.Stop();
            insertTimeLabel.Text = string.Format("Executed in {0} ms", watch.ElapsedMilliseconds.ToString());
            progressBar1.Value = 0;
            executeInsertButton.Enabled = true;
        }

        private async Task InsertSimpleDocuments(IProgress<int> progress)
        {

            //var sqlDictionary = new SqlDictionary(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(Application.ExecutablePath)), "SqlDictionary"));
            var sqlAgent = new MySqlAgent("Server=localhost;Port=3306;Uid=root;Pwd=angelas;CharSet=utf8;", "vnext",
                null, null);
            var service = new MySqlOrmService(sqlAgent);

            

            try
            {

                //var numberTemplate = new ComplexNumberTemplate
                //{
                //    Comments = "some comments",
                //    DocumentType = 3,
                //    ExtendedDocumentTypeId = null,
                //    FormatString = "{1}",
                //    HasExternalProvider = false,
                //    IsArchived = false,
                //    ResetPolicy = ResetPolicyType.never,
                //    Serial = "SER"
                //};

                //await numberTemplate.Save(service);

                //var q = 1;

                //var savedTemplate = await ComplexNumberTemplate.Fetch(service, numberTemplate.Id);

                //q = 2;

                //savedTemplate.Comments = "some modified comments";
                //savedTemplate.DocumentType = 4;
                //savedTemplate.FormatString = "{0}{1}";
                //savedTemplate.ResetPolicy = ResetPolicyType.day;
                //savedTemplate.Serial = "SERI";

                //await savedTemplate.Save(service);

                //q = 2;

                //var allTemplates = await ComplexNumberTemplate.FetchAll(service);

                //q = 2;

                //await savedTemplate.Delete(service);

                //allTemplates = await ComplexNumberTemplate.FetchAll(service);

                //q = 3;
            }
            catch (Exception ex)
            {
                var i = 1;
                throw;
            }

            


            //var account = new Account();
            //account.AccountName = "VAT receivable";
            //account.AccountType = 3;
            //account.BalanceAndIncomeLineId = null;
            //account.Id = 2201L;
            //account.InsertedAt = DateTime.UtcNow;
            //account.InsertedBy = "";
            //account.OfficialCode = "";
            //account.UpdatedAt = account.InsertedAt;
            //account.UpdatedBy = account.InsertedBy;

            //await account.Save(sqlAgent);

            //var q = 1;

            //var savedAccount = await Account.Fetch(sqlAgent, 2201L);

            //q = 2;

            //await savedAccount.Delete(sqlAgent);

            //q = 3;

            //return;


            await sqlAgent.ExecuteInTransactionAsync(async (ct) =>
            {

                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 220L), new SqlParam("AB", "VAT receivable"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 240L), new SqlParam("AB", "Accounts receivable"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 271L), new SqlParam("AB", "Bank account"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 410L), new SqlParam("AB", "Accounts payable"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 445L), new SqlParam("AB", "VAT payable"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 505L), new SqlParam("AB", "Revenus"), new SqlParam("AC", DateTime.UtcNow) });
                await sqlAgent.ExecuteInsertAsync("InsertAccount", new SqlParam[] {
                    new SqlParam("AA", 601L), new SqlParam("AB", "Expenses"), new SqlParam("AC", DateTime.UtcNow) });


                for (int i = 1; i <= 250000; i++)
                {
                    var dayCounter = (int)((250000 - i)/200);
                    await InsertInvoiceMade(DateTime.Today.AddDays(-dayCounter), i, new Random(), sqlAgent, 1);
                    var progressValue = (i+1) / 2500;
                    progress.Report(progressValue);
                }                

            }, CancellationToken.None);
            
        }

        private async Task InsertInvoiceMade(DateTime invoiceDate, int seq, Random rnd, MySqlAgent agent, int version)
        {

            var amount = Math.Round(((Decimal)rnd.Next(10000, 1000000) / 100), 2);
            var amountVat = Math.Round(amount * 0.21m, 2);
            var total = Math.Round(amount + amountVat, 2);
            var paymentDate = invoiceDate.AddDays(rnd.Next(1, 90));
            

            int documentId = (int)await agent.ExecuteInsertAsync("InsertDocument", new SqlParam[] {
                new SqlParam("AA", invoiceDate), new SqlParam("AB", "INV" + seq.ToString()),
                new SqlParam("AC", "Invoice made description seq " + seq.ToString()), new SqlParam("AD", 1),
                new SqlParam("AE", DateTime.UtcNow) });
             int transactionId = (int)await agent.ExecuteInsertAsync("InsertTransaction", new SqlParam[] {
                new SqlParam("AA", documentId), new SqlParam("AB", invoiceDate) });
            await InsertLedgerEntry(transactionId, 505L, false, amount, agent, version);
            await InsertLedgerEntry(transactionId, 445L, false, amountVat, agent, version);
            await InsertLedgerEntry(transactionId, 240L, true, total, agent, version);

            documentId = (int)await agent.ExecuteInsertAsync("InsertDocument", new SqlParam[] {
                new SqlParam("AA", paymentDate), new SqlParam("AB", seq.ToString()),
                new SqlParam("AC", "Payment received description seq " + seq.ToString()), new SqlParam("AD", 2),
                new SqlParam("AE", DateTime.UtcNow) });
            transactionId = (int)await agent.ExecuteInsertAsync("InsertTransaction", new SqlParam[] {
                new SqlParam("AA", documentId), new SqlParam("AB", paymentDate) });
            await InsertLedgerEntry(transactionId, 240L, false, total, agent, version);
            await InsertLedgerEntry(transactionId, 271L, true, total, agent, version);
            
            var amount2 = Math.Min(Math.Round(((Decimal)rnd.Next(10000, 1000000) / 100), 2), amount);
            var amountVat2 = Math.Round(amount2 * 0.21m, 2);
            var total2 = Math.Round(amount2 + amountVat2, 2);
            var paymentDate2 = invoiceDate.AddDays(rnd.Next(1, 90));

            documentId = (int)await agent.ExecuteInsertAsync("InsertDocument", new SqlParam[] {
                new SqlParam("AA", invoiceDate.AddDays(1)), new SqlParam("AB", "REC" + seq.ToString()),
                new SqlParam("AC", "Invoice received description seq " + seq.ToString()), new SqlParam("AD", 3),
                new SqlParam("AE", DateTime.UtcNow) });
            transactionId = (int)await agent.ExecuteInsertAsync("InsertTransaction", new SqlParam[] {
                new SqlParam("AA", documentId), new SqlParam("AB", invoiceDate.AddDays(1)) });
            await InsertLedgerEntry(transactionId, 601L, true, amount2, agent, version);
            await InsertLedgerEntry(transactionId, 220L, true, amountVat2, agent, version);
            await InsertLedgerEntry(transactionId, 410L, false, total2, agent, version);

            documentId = (int)await agent.ExecuteInsertAsync("InsertDocument", new SqlParam[] {
                new SqlParam("AA", paymentDate2), new SqlParam("AB", seq.ToString()),
                new SqlParam("AC", "Payment made description seq " + seq.ToString()), new SqlParam("AD", 2),
                new SqlParam("AE", DateTime.UtcNow) });
            transactionId = (int)await agent.ExecuteInsertAsync("InsertTransaction", new SqlParam[] {
                new SqlParam("AA", documentId), new SqlParam("AB", paymentDate2) });
            await InsertLedgerEntry(transactionId, 410L, true, total2, agent, version);
            await InsertLedgerEntry(transactionId, 271L, false, total2, agent, version);

        }

        private async Task InsertLedgerEntry(int transactionId, long account, bool isDebit, decimal amount, MySqlAgent agent, int version)
        {
            if (version == 1)
            {
                await agent.ExecuteInsertAsync("InsertEntry1", new SqlParam[] { new SqlParam("AA", transactionId),
                    new SqlParam("AB", account), new SqlParam("AC", isDebit?"D":"C"), new SqlParam("AD", amount) });
            }
            else if (version == 2)
            {
                await agent.ExecuteInsertAsync("InsertEntry2", new SqlParam[] { new SqlParam("AA", transactionId),
                    new SqlParam("AB", account), new SqlParam("AC", isDebit?1:0), new SqlParam("AD", amount) });
            }
            else if (version == 3)
            {
                await agent.ExecuteInsertAsync("InsertEntry3", new SqlParam[] { new SqlParam("AA", transactionId),
                    new SqlParam("AB", account), new SqlParam("AC", isDebit?amount:-amount) });
            }
            else if (version == 4)
            {
                decimal? debitAmount, creditAmount;
                if (isDebit)
                {
                    debitAmount = amount;
                    creditAmount = new decimal?();
                }
                else
                {
                    debitAmount = new decimal?();
                    creditAmount = amount;
                }
                await agent.ExecuteInsertAsync("InsertEntry4", new SqlParam[] { new SqlParam("AA", transactionId),
                    new SqlParam("AB", account), new SqlParam("AC", debitAmount), new SqlParam("AD", creditAmount) });
            }
            else
            {
                await agent.ExecuteInsertAsync("InsertEntry1", new SqlParam[] { new SqlParam("AA", transactionId),
                    new SqlParam("AB", account), new SqlParam("AC", isDebit?"Debit":"Credit"), new SqlParam("AD", (long)(amount*100)) });
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {

            var sqlAgent = new MySqlAgent("Server=localhost;Port=3306;Uid=root;Pwd=angelas;CharSet=utf8;", "vnext",
               null, null);

            try
            {
                await sqlAgent.ExecuteInTransactionAsync(async (ct) => 
                {
                    var res = await sqlAgent.ExecuteInTransactionAsync(ThrowingMethod, ct);
                    var i = 11;
                }, CancellationToken.None);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private Task<string> ThrowingMethod(CancellationToken ct) => Task.FromResult("test");

    }
}
