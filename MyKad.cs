using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;
using System.IO;

namespace ServiceConsole
{
    class MyKad
    {
        private static SCardContext m_hcontext;
        private static SCardReader m_reader;
        private static SCardError m_err;
        private static IntPtr m_pioSendPci;

        public static string name;
        public static string origname;
        public static string gpcname;
        public static string icno;
        public static string dob;
        public static string pob;
        public static string gender;
        public static string citizenship;
        public static string religion;
        public static string race;
        public static string addr1;
        public static string addr2;
        public static string addr3;
        public static string poscode;
        public static string city;
        public static string state;
        public static string photo;

        public static void Init()
        {
            // Establish Scard Context
            m_hcontext = new SCardContext();
            m_hcontext.Establish(SCardScope.System);

            // Retrieve the list of Smartcard readers
            string[] szReaders = m_hcontext.GetReaders();
            if (szReaders.Length <= 0)
                throw new PCSCException(SCardError.NoReadersAvailable,
                    "Could not found any Smartcard readers.");

            Console.WriteLine("reader name: " + szReaders[0]);

            // Create a reader object using the existing context
            m_reader = new SCardReader(m_hcontext);

            // Connect to the card
            m_err = m_reader.Connect(szReaders[0],
                SCardShareMode.Shared,
                SCardProtocol.T0 | SCardProtocol.T1);
            CheckErr(m_err);

            switch (m_reader.ActiveProtocol)
            {
                case SCardProtocol.T0:
                    m_pioSendPci = SCardPCI.T0;
                    break;
                case SCardProtocol.T1:
                    m_pioSendPci = SCardPCI.T1;
                    break;
                default:
                    throw new PCSCException(SCardError.ProtocolMismatch,
                        "Protocol not supported: "
                        + m_reader.ActiveProtocol.ToString());
            }
        }

        public static void SelectJPN()
        {
            string s1, s2;
            s1 = APCOM.SELECT_JPN_APPLICATION.Replace(" ", "");
            s2 = APCOM.SELECT_APPLICATION_GET_RESPONSE.Replace(" ", "");

            //Preparing apdu cmd
            byte[][] send = { hexToByteArr(s1), hexToByteArr(s2) };
            try
            {
                byte[] response = new byte[256];
                for (int t = 0; t < send.Length; t++)
                {
                    response = new byte[256];

                    // Send SELECT command
                    m_err = m_reader.Transmit(m_pioSendPci, send[t], ref response);
                    //Console.Write("send: ");
                    //for (int k = 0; k < send[t].Length; k++)
                    //  Console.Write("{0:X2} ", send[t][k]);
                    //Console.WriteLine();
                    
                    CheckErr(m_err);

                    //Console.Write("response: ");
                    //for (int i = 0; i < response.Length; i++)
                    //    Console.Write("{0:X2} ", response[i]);
                    //Console.WriteLine();

                    Array.Clear(response, 0, 0);
                }
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Ouch: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
            }
        }

        public static string GetFld(string[] fld, string pg)
        {
            //[SETLENGTH(8)][len(2)]
            //[SELECTINFO(5)][JPNPAGE(4)][offset(2)][len(2)]
            //[READINFO(4)][len(1)]

            string s1, s2, s3, rs="";
            s1 = APCOM.SET_LENGTH + fld[0] + "00";
            s1 = s1.Replace(" ", "");

            s2 = APCOM.SELECT_INFO + pg + fld[1] + fld[0] + "00";
            s2 = s2.Replace(" ", "");

            s3 = APCOM.READ_INFO + fld[0];
            s3 = s3.Replace(" ", "");

            //Preparing apdu cmd
            byte[][] send = { hexToByteArr(s1), hexToByteArr(s2), hexToByteArr(s3) };
            try
            {
                byte[] response = new byte[256];
                for (int t = 0; t < send.Length; t++)
                {
                    response = new byte[256];

                    // Send SELECT command
                    m_err = m_reader.Transmit(m_pioSendPci, send[t], ref response);
                    //Console.Write("send: ");
                    //for (int k = 0; k < send[t].Length; k++)
                    //    Console.Write("{0:X2} ", send[t][k]);
                    //Console.WriteLine();

                    CheckErr(m_err);

                    //Console.Write("response: ");
                    //for (int i = 0; i < response.Length; i++)
                    //    Console.Write("{0:X2} ", response[i]);
                    //Console.WriteLine();

                    Array.Clear(response, 0, 0);
                }
                string check = BitConverter.ToString(response).Replace("-","");
                if (!check.Contains("9000"))
                {
                    Console.WriteLine("Opps.. Getting fld failed.");
                    return "";
                }

                string test = ba2s(response);
                test = test.Replace("-", "");

                byte[] res = new byte[response.Length - 2];
                Array.Copy(response, res, response.Length - 2);
                rs = Encoding.UTF8.GetString(res);
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Ouch: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
            }

            return rs;
        }

        public static string GetFldNot(string[] fld, string pg, int need)
        {
            //[SETLENGTH(8)][len(2)]
            //[SELECTINFO(5)][JPNPAGE(4)][offset(2)][len(2)]
            //[READINFO(4)][len(1)]

            string s1, s2, s3, rs = "";
            s1 = APCOM.SET_LENGTH + fld[0] + "00";
            s1 = s1.Replace(" ", "");

            s2 = APCOM.SELECT_INFO + pg + fld[1] + fld[0] + "00";
            s2 = s2.Replace(" ", "");

            s3 = APCOM.READ_INFO + fld[0];
            s3 = s3.Replace(" ", "");

            //Preparing apdu cmd
            byte[][] send = { hexToByteArr(s1), hexToByteArr(s2), hexToByteArr(s3) };
            try
            {
                byte[] response = new byte[256];
                for (int t = 0; t < send.Length; t++)
                {
                    response = new byte[256];

                    // Send SELECT command
                    m_err = m_reader.Transmit(m_pioSendPci, send[t], ref response);
                    //Console.Write("send: ");
                    //for (int k = 0; k < send[t].Length; k++)
                    //    Console.Write("{0:X2} ", send[t][k]);
                    //Console.WriteLine();

                    CheckErr(m_err);
                    
                    if (t == 2)
                    {
                        //Console.Write("response: ");
                        for (int i = 0; i < response.Length; i++)
                        {
                            //Console.Write("{0:X2} ", pbRecvBuffer[i]);
                            rs = rs + string.Format("{0:X2}", response[i]);
                        }
                    }

                    Array.Clear(response, 0, 0);
                }
                string check = BitConverter.ToString(response).Replace("-", "");
                if (!check.Contains("9000"))
                {
                    Console.WriteLine("Opps.. Getting fld failed.");
                    return "";
                }

                rs = rs.Substring(0, need);
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Ouch: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
            }

            return rs;
        }
        
        public static string GetPhoto()
        {
            //[SETLENGTH(8)][len(2)]
            //[SELECTINFO(5)][JPNPAGE(4)][offset(2)][len(2)]
            //[READINFO(4)][len(1)]

            int len = 0xff, offset = 0x03;
            int max = 4000;

            string s1, s2, s3, res ="", sLen, sOff;
            bool go = true;
            string lastStr = "";
            
            do
            {
                if ((offset + len) > max)
                    len = (max - offset);
                else
                    len = 0xff;
                
                sLen = len.ToString("X"); //FF
                //Console.WriteLine("Remaining Length:" + sLen);
                string tmp = offset.ToString("X2");
                //Console.WriteLine("Tmp Offset:" + tmp);
                if(tmp.Length <= 2)
                    sOff = tmp + "00"; //0300
                else
                    sOff = tmp.Substring(1,2) +" 0" + tmp.Substring(0,1); //3000

                //Console.WriteLine("Current Offset: " + sOff);               
                s1 = APCOM.SET_LENGTH + sLen+ "00"; 
                s1 = s1.Replace(" ", "");
                //Console.WriteLine(s1);

                s2 = APCOM.SELECT_INFO + APCOM.JPN[1] + sOff + sLen+"00";
                s2 = s2.Replace(" ", "");
                //Console.WriteLine(s2);

                s3 = APCOM.READ_INFO + sLen;
                s3 = s3.Replace(" ", "");
                //Console.WriteLine(s3);

                //Preparing apdu cmd
                byte[][] send = { hexToByteArr(s1), hexToByteArr(s2), hexToByteArr(s3) };

                try
                {
                    byte[] response = new byte[256];
                    for (int t = 0; t < send.Length; t++)
                    {
                        if(t==2)
                            response = new byte[300];
                        else
                            response = new byte[256];

                        // Send SELECT command
                        m_err = m_reader.Transmit(m_pioSendPci, send[t], ref response);

                        //Console.Write("send: ");
                        //for (int k = 0; k < send[t].Length; k++)
                        //    Console.Write("{0:X2} ", send[t][k]);
                        //Console.WriteLine();

                        CheckErr(m_err);

                        //Console.Write("response: ");
                        //for (int i = 0; i < response.Length; i++)
                        //    Console.Write("{0:X2} ", response[i]);
                        //Console.WriteLine();

                        if (t == 2)
                        {/*
                            Console.Write("send: ");
                            for (int k = 0; k < send[t].Length; k++)
                                Console.Write("{0:X2} ", send[t][k]);
                            Console.WriteLine();
                            */
                            CheckErr(m_err);
/*
                            Console.Write("response: ");
                            for (int i = 0; i < response.Length; i++)
                                Console.Write("{0:X2} ", response[i]);
                            Console.WriteLine();
                            */
                            len = response.Length-2; //remove 9000

                            res = ba2s(response).Replace("-", "");
                            
                            //Console.WriteLine(res);
                            if (res.Contains("FFD9")) //last bytes for photo
                                go = false;

                            res = res.Replace("9000", "");
                            lastStr += res;
                        }
                        
                        Array.Clear(response, 0, 0);
                    }
                    string check = BitConverter.ToString(response).Replace("-", "");
                    if (!check.Contains("9000"))
                    {
                        Console.WriteLine("Opps.. Getting fld failed.");
                        return null;
                    }
                }
                catch (PCSCException ex)
                {
                    Console.WriteLine("Ouch: "
                        + ex.Message
                        + " (" + ex.SCardError.ToString() + ")");
                }
                
                if ((offset + len) > max)
                    offset += (max - offset);
                else
                    offset += len;
            }
            while (go);

            lastStr = lastStr.Trim('0');
    
            byte[] imgBuf;
            imgBuf = hexToByteArr(lastStr + "9"); //append missing niner

            string base64 = Convert.ToBase64String(imgBuf);
            
            photo = base64;
            return photo;
        }

        public static byte[] hexToByteArr(string hex)
        {
            if (hex.Length % 2 != 0)
                return null;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string ba2s(byte[] data)
        {
            return BitConverter.ToString(data);
        }

        public static void Flush()
        {
            m_hcontext.Cancel();

            if (m_reader != null)
            {
                m_reader.Disconnect(SCardReaderDisposition.Unpower);
                Console.WriteLine("Card Dispose.");
            }
        }

        public static void ReadInfo()
        {
            try
            {
                if (m_hcontext != null)
                {
                    Console.WriteLine("Retrieving info started ...");
                    string JPN1 = APCOM.JPN[0];
                    string JPN4 = APCOM.JPN[3];

                    //Page JPN-1-1
                    name = GetFld(APCOM.NAME, JPN1);
                    origname = GetFld(APCOM.ORIG_NAME, JPN1);
                    gpcname = GetFld(APCOM.GPC_NAME, JPN1);
                    icno = GetFld(APCOM.IC_NUMBER, JPN1);
                    dob = GetFldNot(APCOM.DOB, JPN1, 8);
                    gender = GetFld(APCOM.GENDER, JPN1);
                    pob = GetFld(APCOM.BIRTH_PLACE, JPN1);
                    race = GetFld(APCOM.RACE, JPN1);
                    citizenship = GetFld(APCOM.CITIZENSHIP, JPN1);
                    religion = GetFld(APCOM.RELIGION, JPN1);

                    addr1 = GetFld(APCOM.ADDRESS_1, JPN4);
                    addr2 = GetFld(APCOM.ADDRESS_2, JPN4);
                    addr3 = GetFld(APCOM.ADDRESS_3, JPN4);
                    poscode = GetFldNot(APCOM.POSCODE, JPN4, 5);
                    city = GetFld(APCOM.CITY, JPN4);
                    state = GetFld(APCOM.STATE, JPN4);
                    photo = GetPhoto();
                }
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Ouch: "
                    + ex.Message
                    + " (" + ex.SCardError.ToString() + ")");
            }
        }

        public static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success)
                throw new PCSCException(err,
                    SCardHelper.StringifyError(err));
            
        }
        
        
    }

    class APCOM
    {
        public static string[] NAME = { "28", "E9 00" };
        public static string[] ORIG_NAME = { "96" , "03 00" };
        public static string[] GPC_NAME = { "50", "99 00" };
        public static string[] IC_NUMBER = { "0D", "11 01" };
        public static string[] GENDER = { "01", "1E 01" };
        public static string[] OLD_IC = { "08", "1F 01" };
        public static string[] DOB = { "04", "27 01" };
        public static string[] BIRTH_PLACE = { "19", "2B 01" };
        public static string[] CITIZENSHIP = { "12", "48 01" };
        public static string[] RACE = { "19", "5A 01" };
        public static string[] RELIGION = { "0B", "73 01" };

        public static string[] ADDRESS_1 = { "1E", "03 00" };
        public static string[] ADDRESS_2 = { "1E", "21 00" };
        public static string[] ADDRESS_3 = { "1E", "3F 00" };
        public static string[] POSCODE = { "03", "5D 00" };
        public static string[] CITY = { "19", "60 00" };
        public static string[] STATE = { "1E", "79 00" };

        public static string[] THUMBPRINT_RIGHT = { "FF", "17 00" };
        public static string[] THUMBPRINT_LEFT = { "FF", "6D 02" };

        public static string[] PHOTO = { "FF", "03 00"};

        public static string ATR = "3B 67 00 00 73 20 00 6C 68 90 00";
        public static string SELECT_JPN_APPLICATION = "00 A4 04 00 0A A0 00 00 00 74 4A 50 4E 00 10";
        public static string SELECT_APPLICATION_GET_RESPONSE = "00 C0 00 00 05";

        public static string SET_LENGTH = "C8 32 00 00 05 08 00 00";
        public static string SELECT_INFO = "CC 00 00 00 08";
        public static string READ_INFO = "CC 06 00 00";

        public static string[] JPN = { "01 00 01 00", "02 00 01 00", "03 00 01 00",
                                      "04 00 01 00", "05 00 01 00", "06 00 01 00"};
    }
}
