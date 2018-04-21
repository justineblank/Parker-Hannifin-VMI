using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RFIDApplication.DAL;
using RFIDApplication.DAL.Interfaces;
using RFIDApplication.DAL.Models;
using RFIDApplication.DAL.Repositories;

namespace RFIDApplication.Controllers.API
{
    //[ResponseCache(CacheProfileName = "Never")]    
    [Route("api/[controller]")]
    public class RFID : Controller
    {
        private readonly IRFIDPayloadRepository _repository;
        private readonly IReadersRepository _repositoryReader;
        private readonly IScansRepository _repositoryScans;
        public RFID(IRFIDPayloadRepository repository,
            IReadersRepository repositoryReader,
            IScansRepository repositoryScans)
        {
            _repository = repository;
            _repositoryReader = repositoryReader;
            _repositoryScans = repositoryScans;
        }
        //POST: api/RFID
        /// <summary>
        /// POST: api/RFID
        /// add RFID data to scans value, add new RFID or Update RFID lastseen status 
        /// </summary>
        /// <returns datatype="json">
        ///     {
        ///         "status": "Success/Fail",
        ///         "message": "if 'status' is Fail",
        ///         "tagCount": @tagCount,
        ///         "tags": 
        ///         [
        ///             {
        ///                 "readId": "@readId1",
        ///                 "status": "Success/Fail",
        ///                 "message": "if 'status' is Fail"
        ///             },
        ///             {
        ///                 "readId": "@readId2",
        ///                 "status": "Success/Fail",
        ///                 "message": "if 'status' is Fail"
        ///             }
        ///         ]
        ///     }
        /// </returns>
        /// <param name="RFIDPayload" datatype="json">
        ///     {"readerId": "@readerId",
        ///	    "tagCount": @tagCount,
        ///	    "tags":
        ///         [
        ///         {
        ///			    "readId"    : @readId1,
        ///			    "epc"       : "@epc1",
        ///			    "timestamp" : @timestamp1,
        ///			    "antenna"   : @antenna1
        ///        },
        ///		   {
        ///		    	"readId"    : @readId2,
        ///			    "epc"       : "@epc2",
        ///			    "timestamp" : timestamp2,
        ///			    "antenna"   : antenna2
        ///		    }
        ///         ,
        ///         ...
        ///	        ]
        ///     }
        /// 
        /// </param>
        /// <example>
        ///     Request:
        ///         POST: api/RFID
        ///     
        ///     Request content:
        ///     {
        ///         "readerId": "PL:US:TE:ST:40:56",
        ///	        "tagCount": 2,
        ///	        "tags": 
        ///         [
        ///             {
        ///			        "readId": 1234567892,
        ///			        "epc": "PLUSEF1234567890",
        ///			        "timestamp": 1521010758,
        ///			        "antenna": 0
        ///             },
        ///             {
        ///			        "readId": 1234567893,
        ///			        "epc": "PLUSEF1234567890",
        ///			        "timestamp": 1521010758,
        ///			        "antenna": 0
        ///		        }
        ///	        ]
        ///     } 
        ///
        ///     Response:
        ///      {
        ///         "status": "Success",
        ///         "message": "",
        ///         "tagCount": 2,
        ///         "tags": 
        ///         [
        ///             {
        ///                "readId": "1234567892",
        ///                "status": "success",
        ///                "message": ""
        ///             },
        ///             {
        ///                "readId": "1234567893",
        ///                "status": "success",
        ///                "message": ""
        ///             }
        ///         ]
        ///     }
        /// </example>
        [HttpPost]
        public async Task<RFIDPayloadResponseModel> Post([FromBody]RFIDPayloadModel RFIDPayload)
        {
            RFIDPayloadResponseModel returnModel = new RFIDPayloadResponseModel();
            //TagsResponse tagsResponse = null;
            //


            if (RFIDPayload == null)
            {
                //string[] messages = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray();
                //returnModel.ValidationMessage = await _labelService.ResolveMessagesAsync(messages);
                returnModel.status = "Fail";
                returnModel.message = "data is not in valid format";
                return returnModel;
            }
            else
            {
                //List<Tags> lst_Tags = model.tags;
                try
                {
                    returnModel = await _repository.CreateAsync(RFIDPayload);

                }
                catch (Exception ex)
                {
                    returnModel.status = "Fail";
                    returnModel.message = ex.ToString();
                }
            }
            return returnModel;
        }


        // GET: api/RFID/Readers
        /// <summary>
        /// GET: api/RFID/Readers
        /// All readers detail order by last seen descending status, in JSON data format
        /// </summary>
        /// <returns datatype="json"> 
        ///    [{
        ///        "id": @id1,
        ///        "readerId": "@readerId1",
        ///        "location": "@location1",
        ///        "lastSeen": "@lastSeen1"
        ///    },
        ///    {
        ///        "id": @id2,
        ///        "readerId": "@readerId2",
        ///        "location": "@location2",
        ///        "lastSeen": "@lastSeen2"
        ///    },
        ///    ...]
        /// </returns>
        /// <example>
        ///     Requst:
        ///     GET: api/RFID/Readers
        ///     
        ///     Response:
        ///     [{
        ///        "id": 6,
        ///        "readerId": "PL:US:TE:ST:40:56",
        ///        "location": "CNH-St. Nazianz - Shop Floor",
        ///        "lastSeen": "2018-03-14T00:11:18.753"
        ///      },
        ///      {
        ///        "id": 5,
        ///        "readerId": "00:0d:29:e0:40:56",
        ///        "location": "CNH-GreenBay - Line 4",
        ///        "lastSeen": "2018-03-14T00:08:32.547"
        ///      }]
        /// </example>
        [Route("Readers")]
        [HttpGet]
        public async Task<IEnumerable<ReadersModel>> Readers()
        {
            IEnumerable<ReadersModel> returnModel = null;

            try
            {
                returnModel = await _repositoryReader.ReadAsync();
            }
            catch (Exception ex)
            {

            }

            return returnModel;
        }



        // GET: api/RFID/Scans
        /// <summary>
        /// GET: api/RFID/Scans
        /// All scans detail order by time scanned descending, in JSON data format
        /// </summary>
        /// <returns datatype="json">          
        ///    [{
        ///        "id"         :  @id1,
        ///        "readerId"   : "@readerId1",
        ///        "antenna"    :  @antenna1,
        ///        "epc"        : "@epc1",
        ///        "timestamp"  : "@timestamp1",
        ///        "syncStatus" : "@syncStatus1",
        ///        "message"    : "@message1",
        ///        "location"   : "@location1",
        ///        "lastSeen"   : "@lastSeen1"
        ///    },
        ///    {
        ///        "id"         :  @id1,
        ///        "readerId"   : "@readerId2",
        ///        "antenna"    :  @antenna2,
        ///        "epc"        : "@epc2",
        ///        "timestamp"  : "@timestamp2",
        ///        "syncStatus" : "@syncStatus2",
        ///        "message"    : "@message2",
        ///        "location"   : "@location2",
        ///        "lastSeen"   : "@lastSeen2"
        ///    },
        ///    ...
        ///    ]
        /// </returns>
        /// <example>
        ///     Requst:
        ///     GET: api/RFID/Scans
        ///     
        ///     Response:
        ///     [{
        ///        "id"         : 12,
        ///        "readerId"   : "00:0c:29:e0:40:57",
        ///        "antenna"    : 0,
        ///        "epc"        : "ABCDEF1234567890",
        ///        "timestamp"  : "2451-11-19T14:07:31",
        ///        "syncStatus" : "",
        ///        "message"    : "",
        ///        "location"   : "Shopfloor B2, Location X",
        ///        "lastSeen"   : "2018-03-10T03:49:11.313"
        ///      },
        ///      {
        ///        "id"         : 20,
        ///        "readerId"   : "00:16:25:11:FB:CC",
        ///        "antenna"    : 1,
        ///        "epc"        : "CA08000000000040000004A3",
        ///        "timestamp"  : "2018-03-14T00:33:15",
        ///        "syncStatus" : "",
        ///        "message"    : "",
        ///        "location"   : "Parker Manitowoc PSC",
        ///        "lastSeen"   : "2018-03-11T18:26:11.047"
        ///      }]
        /// </example>        
        [Route("Scans")]
        [HttpGet]
        public async Task<IEnumerable<ScansModel>> Scans()
        {
            IEnumerable<ScansModel> returnModel = null;

            try
            {
                returnModel = await _repositoryScans.ReadAsync();
            }
            catch (Exception ex)
            {

            }

            return returnModel;
        }

        // POST: api/RFID/ReadersEdit
        /// <summary>
        /// POST: api/RFID/ReadersEdit
        /// Edit single reader's location and returns modified reader detail in JSON data format
        /// </summary>
        /// <param name="ReadersEdit" datatype="application/json">
        ///     {
        ///         "id"        :  @id ,
        ///         "location"  : "@location",
        ///         "readerId"  : "@readerId"
        ///     }
        /// </param>
        /// <returns datatype="json">
        ///     {
        ///        "id": @id,
        ///        "readerId": "@readerId",
        ///        "location": "@location",
        ///        "lastSeen": "@lastSeen"
        ///     }
        /// </returns>
        /// <example>
        ///     Request:
        ///     POST: api/RFID/ReadersEdit
        ///     
        ///     Request content:
        ///     {
        ///         "id"        :  12 ,
        ///         "location"  : "5th - Shop Floor",
        ///         "readerId"  : "00:0d:29:e0:40:56"
        ///     }
        ///     
        ///     Response:
        ///      {
        ///        "id": 12,
        ///        "readerId": "00:0d:29:e0:40:56",
        ///        "location": "5th - Shop Floor",
        ///        "lastSeen": "2018-03-14T00:11:18.753"
        ///      }
        /// </example>
        [Route("ReadersEdit")]
        [HttpPost]
        public async Task<ReadersModel> ReadersEdit([FromBody]ReadersEditModel ReadersEdit)
        {
            ReadersModel returnModel = null;
            try
            {
                returnModel = await _repositoryReader.ReadersEditAsync(ReadersEdit);
            }
            catch (Exception ex)
            {

            }

            return returnModel;
        }
    }
}
