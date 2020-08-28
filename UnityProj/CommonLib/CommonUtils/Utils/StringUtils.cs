
namespace Nullspace
{
    public static class StringUtils
    {
        private static string Activestring = null;
        private static int Activeposition = 0;
        public static string StrTok(string stringtotokenize, string delimiters)
        {
            if (stringtotokenize != null)
            {
                Activestring = stringtotokenize;
                Activeposition = -1;
            }

            //the stringtotokenize was never set:
            if (Activestring == null)
            {
                return null;
            }

            //all tokens have already been extracted:
            if (Activeposition == Activestring.Length)
            {
                return null;
            }

            //bypass delimiters:
            Activeposition++;
            while (Activeposition < Activestring.Length && delimiters.IndexOf(Activestring[Activeposition]) > -1)
            {
                Activeposition++;
            }

            //only delimiters were left, so return null:
            if (Activeposition == Activestring.Length)
            {
                return null;
            }

            //get starting position of string to return:
            int startingposition = Activeposition;

            //read until next delimiter:
            do
            {
                Activeposition++;
            } while (Activeposition < Activestring.Length && delimiters.IndexOf(Activestring[Activeposition]) == -1);

            return Activestring.Substring(startingposition, Activeposition - startingposition);
        }
    }
}
