using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Imposto.Core.Domain
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int Serie { get; set; }
        public string NomeCliente { get; set; }

        public string EstadoDestino { get; set; }
        public string EstadoOrigem { get; set; }

        public IEnumerable<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();
        }
        
        public void GerarXML(Pedido pedido)
        {
            XmlSerializer serializer =
                    new XmlSerializer(typeof(NotaFiscal));

            var filename = @"C:\NetShoes\XML\Teste.xml"; 

            var notaFiscal = new NotaFiscal();
            notaFiscal.NumeroNotaFiscal = 99999;
            notaFiscal.Serie = new Random().Next(Int32.MaxValue);
            notaFiscal.NomeCliente = pedido.NomeCliente;
            notaFiscal.EstadoDestino = pedido.EstadoOrigem;
            notaFiscal.EstadoOrigem = pedido.EstadoDestino;

            // Create a FileStream to write with.
            FileStream writer = File.Create(filename);

            // Serialize the object, and close the TextWriter
            serializer.Serialize(writer, notaFiscal);
            writer.Close();
        }

        public void EmitirNotaFiscal(Pedido pedido)
        {
            this.NumeroNotaFiscal = 99999;
            this.Serie = new Random().Next(Int32.MaxValue);
            this.NomeCliente = pedido.NomeCliente;

            this.EstadoDestino = pedido.EstadoDestino;
            this.EstadoOrigem = pedido.EstadoOrigem;

            string connectionString = @"Data Source=localhost;Initial Catalog=TESTE;User Id=sa;Password=gerente";

            #region Grava NF

            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand("[dbo].[P_NOTA_FISCAL]", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@pId", SqlDbType.Int)).Value = this.Id;
                    command.Parameters.Add(new SqlParameter("@pNumeroNotaFiscal", SqlDbType.Int)).Value = this.NumeroNotaFiscal;
                    command.Parameters.Add(new SqlParameter("@pSerie", SqlDbType.Int)).Value = this.Serie;
                    command.Parameters.Add(new SqlParameter("@pNomeCliente", SqlDbType.VarChar)).Value = this.NomeCliente;
                    command.Parameters.Add(new SqlParameter("@pEstadoDestino", SqlDbType.VarChar)).Value = this.EstadoDestino;
                    command.Parameters.Add(new SqlParameter("@pEstadoOrigem", SqlDbType.VarChar)).Value = this.EstadoOrigem;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            #endregion

            foreach (PedidoItem itemPedido in pedido.ItensDoPedido)
            {
                NotaFiscalItem notaFiscalItem = new NotaFiscalItem();

                string _OriDes = this.EstadoOrigem + this.EstadoDestino;

                #region Cálculo de CFOP
                
                switch (_OriDes)
                {
                    case "SPRJ":
                        notaFiscalItem.Cfop = "6.000";
                        break;
                    case "SPPE":
                        notaFiscalItem.Cfop = "6.001";
                        break;
                    case "SPMG":
                        notaFiscalItem.Cfop = "6.002";
                        break;
                    case "SPPB":
                        notaFiscalItem.Cfop = "6.003";
                        break;
                    case "SPPR":
                        notaFiscalItem.Cfop = "6.004";
                        break;
                    case "SPPI":
                        notaFiscalItem.Cfop = "6.005";
                        break;
                    case "SPRO":
                        notaFiscalItem.Cfop = "6.006";
                        break;
                    case "SPSE":
                        notaFiscalItem.Cfop = "6.007";
                        break;
                    case "SPTO":
                        notaFiscalItem.Cfop = "6.008";
                        break;
                    case "SPPA":
                        notaFiscalItem.Cfop = "6.010";
                        break;
                    case "MGRJ":
                        notaFiscalItem.Cfop = "6.000";
                        break;
                    case "MGPE":
                        notaFiscalItem.Cfop = "6.001";
                        break;
                    case "MGMG":
                        notaFiscalItem.Cfop = "6.002";
                        break;
                    case "MGPB":
                        notaFiscalItem.Cfop = "6.003";
                        break;
                    case "MGPR":
                        notaFiscalItem.Cfop = "6.004";
                        break;
                    case "MGPI":
                        notaFiscalItem.Cfop = "6.005";
                        break;
                    case "MGRO":
                        notaFiscalItem.Cfop = "6.006";
                        break;
                    case "MGSE":
                        notaFiscalItem.Cfop = "6.007";
                        break;
                    case "MGTO":
                        notaFiscalItem.Cfop = "6.008";
                        break;
                    case "MGPA":
                        notaFiscalItem.Cfop = "6.010";
                        break;
                    default:
                        notaFiscalItem.Cfop = "0.000";
                        break;
                }

                #endregion

                if (this.EstadoDestino == this.EstadoOrigem)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                }
                else
                {
                    notaFiscalItem.TipoIcms = "10";
                    notaFiscalItem.AliquotaIcms = 0.17;
                }

                if (notaFiscalItem.Cfop == "6.009")
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido * 0.90; //redução de base
                }
                else
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido;
                }

                notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms*notaFiscalItem.AliquotaIcms;

                if (itemPedido.Brinde)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                    notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;
                    notaFiscalItem.AliquotaIpi = 0;
                }
                else
                {
                    notaFiscalItem.AliquotaIpi = 0.1;
                }

                notaFiscalItem.NomeProduto = itemPedido.NomeProduto;
                notaFiscalItem.CodigoProduto = itemPedido.CodigoProduto;

                notaFiscalItem.BaseIpi = itemPedido.ValorItemPedido;
                notaFiscalItem.ValorIpi = notaFiscalItem.BaseIpi * notaFiscalItem.AliquotaIpi;

                if (this.EstadoDestino == "SP" || this.EstadoDestino == "RJ" ||
                    this.EstadoDestino == "MG" || this.EstadoDestino == "ES")
                {
                    notaFiscalItem.Desconto = 0.1;
                }
                else
                {
                    notaFiscalItem.Desconto = 0.0;
                }
                
                #region Grava Item NF

                using (var conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("[dbo].[P_NOTA_FISCAL_ITEM]", conn);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@pId", SqlDbType.Int)).Value = notaFiscalItem.Id;
                        command.Parameters.Add(new SqlParameter("@pIdNotaFiscal", SqlDbType.Int)).Value = notaFiscalItem.IdNotaFiscal;
                        command.Parameters.Add(new SqlParameter("@pCfop", SqlDbType.VarChar)).Value = notaFiscalItem.Cfop;
                        command.Parameters.Add(new SqlParameter("@pTipoIcms", SqlDbType.VarChar)).Value = notaFiscalItem.TipoIcms;
                        command.Parameters.Add(new SqlParameter("@pBaseIcms", SqlDbType.Decimal)).Value = notaFiscalItem.BaseIcms;
                        command.Parameters.Add(new SqlParameter("@pAliquotaIcms", SqlDbType.Decimal)).Value = notaFiscalItem.AliquotaIcms;
                        command.Parameters.Add(new SqlParameter("@pValorIcms", SqlDbType.Decimal)).Value = notaFiscalItem.ValorIcms;
                        command.Parameters.Add(new SqlParameter("@pNomeProduto", SqlDbType.VarChar)).Value = notaFiscalItem.NomeProduto;
                        command.Parameters.Add(new SqlParameter("@pCodigoProduto", SqlDbType.VarChar)).Value = notaFiscalItem.CodigoProduto;
                        command.Parameters.Add(new SqlParameter("@pBaseIpi", SqlDbType.Decimal)).Value = notaFiscalItem.BaseIpi;
                        command.Parameters.Add(new SqlParameter("@pAliquotaIpi", SqlDbType.Decimal)).Value = notaFiscalItem.AliquotaIpi;
                        command.Parameters.Add(new SqlParameter("@pValorIpi", SqlDbType.Decimal)).Value = notaFiscalItem.ValorIpi;
                        command.Parameters.Add(new SqlParameter("@pDesconto", SqlDbType.Decimal)).Value = notaFiscalItem.Desconto;
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                #endregion

            }
        }
    }
}
