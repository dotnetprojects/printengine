using System;
using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Xml;

namespace SUT.PrintEngine.Utils
{
    public static class XpsPrinterUtils
    {
        public static Dictionary<string, string> GetInputBins(PrintQueue printQueue)
        {
            var inputBins = new Dictionary<string, string>();

            var printerCapXmlStream = printQueue.GetPrintCapabilitiesAsXml();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(printerCapXmlStream);

            var manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace(xmlDoc.DocumentElement.Prefix, xmlDoc.DocumentElement.NamespaceURI);

            var nodeList = xmlDoc.SelectNodes("//psf:Feature[@name='psk:JobInputBin']/psf:Option", manager);

            foreach (XmlNode node in nodeList)
            {
                inputBins.Add(node.LastChild.InnerText, node.Attributes["name"].Value);
            }

            return inputBins;
        }


        public static PrintTicket ModifyPrintTicket(PrintTicket ticket, string featureName, string newValue)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("ticket");
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(ticket.GetXmlStream());

            var manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace(xmlDoc.DocumentElement.Prefix, xmlDoc.DocumentElement.NamespaceURI);

            var xpath = string.Format("//psf:Feature[contains(@name, 'InputBin')]/psf:Option", featureName);
            var node = xmlDoc.SelectSingleNode(xpath, manager);
            if (node != null)
            {
                node.Attributes["name"].Value = newValue;
            }

            var printTicketStream = new MemoryStream();
            xmlDoc.Save(printTicketStream);
            printTicketStream.Position = 0;
            var modifiedPrintTicket = new PrintTicket(printTicketStream);
            return modifiedPrintTicket;
        }
    }
}
