using System;
using System.Diagnostics;

namespace Fjord.Modules.Debug {
    public static class Debug
    {
        public static string last_message = "";
        public static int last_message_streak = 0;

        public static void assert(bool condition, string message) {
            if(!condition) {
                error(message, true);
            }
        }

        #nullable enable
        public static void send(dynamic message, string? funcoverride=null, string? prefix=null) {
            message = message.ToString();
            
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            string method;

            if(funcoverride == null) 
                if(sf is not null)
                    method = sf.GetMethod()!.Name;
                else
                    method = "";
            else
                method = funcoverride;
            
            string prefixstr;
            
            if(prefix == null) 
                prefixstr = "";
            else
                prefixstr = "[" + prefix + "]";

            string time = DateTime.Now.ToString("HH:mm:ss");
            if(message != last_message) {
                string msg = String.Format("[{0}]{1} {2} -> {3}", time, prefixstr, method, message);
                Console.WriteLine(msg);

                game.log.Add(msg);  

                last_message_streak = 0;
            } else {
                Console.SetCursorPosition(0, Console.CursorTop -1);

                string msg = String.Format(prefixstr + "[{0}]{1} {2}x {3} -> {4}", time, prefixstr, (last_message_streak + 2).ToString(), method, message);
                Console.WriteLine(msg); 

                game.log[game.log.Count - 1] = msg;
                
                last_message_streak += 1;
            }  
            last_message = message;       
        }
        #nullable disable

        public static void error(dynamic message, bool stop=true) {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            
            if(sf is not null)
                send(message, sf.GetMethod()!.Name, "Error");
            else
                send(message, "", "Error");
            
            if(stop)
                game.stop(1);
        }

        public static void warn(dynamic message) {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            if(sf is not null)
                send(message, sf.GetMethod()!.Name, "Warn");
            else
                send(message, "", "Warn");
        }
    }
}