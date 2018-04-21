using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RFIDApplication.DAL.Models;

namespace RFIDApplication.DAL.Repositories
{
    public abstract class BaseMessageRepository<T> where T : new()
	{
		//Implements IDisposable
		//protected readonly IConfigurationRoot _config;
        private readonly IStringLocalizer<T> _localizer;
        
        public BaseMessageRepository(IStringLocalizer<T> localizer)
		{
			//_config = config;
			_localizer = localizer;
			
		}

        protected async Task<string> ResolveMessage(string message)
        {
            //does not yet work with multiple label keys in message
            string resolvedMessage = _localizer[message];

            int tagStartIndex = resolvedMessage.IndexOf("<%");

            if (tagStartIndex >= 0)
            {
                int labelNameStartIndex = tagStartIndex + 2;
                int labelNameEndIndex = resolvedMessage.IndexOf("%>");

                string labelKey = message.Substring(labelNameStartIndex, labelNameEndIndex - labelNameStartIndex);

                //LabelModel label = await _labelRepository.ReadAsync(labelKey);

                //if (label != null)
                //{
                //    resolvedMessage = resolvedMessage.Replace("<%" + labelKey + "%>", label.Translation);
                //}
            }

            return resolvedMessage;
        }

        //protected async Task<string> ResolveMessage_RegEx(string message)
        //{
        //    //not working correctly yet with multiple label keys in message
        //    string resolvedMessage = _localizer[message];

        //    int startLabelNameStartIndex = resolvedMessage.IndexOf("<%");

        //    MatchCollection matches = Regex.Matches(message, @"<%(.*)\%>", RegexOptions.Singleline);

        //    if (matches.Count > 0)
        //    {
        //        foreach (Match match in matches) {
        //            string labelKey = match.Groups[1].Value;

        //            LabelModel label = await _labelRepository.ReadAsync(labelKey);

        //            if (label != null)
        //            {
        //                resolvedMessage = resolvedMessage.Replace("<%" + labelKey + "%>", label.Translation);
        //            }
        //        }
        //    }

        //    return resolvedMessage;
        //}
    }
}
