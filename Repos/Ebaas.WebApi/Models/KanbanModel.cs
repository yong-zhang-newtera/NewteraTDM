using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ebaas.WebApi.Models
{
    /// <summary>
    /// Kanban State model
    /// </summary>
    public class KanbanStateModel
    {
        // Unique State ID
        public string id { get; set; }

        // State display name
        public string name { get; set; }

        // Whether to allow adding item to this state
        public bool areNewItemButtonsHidden { get; set; }
    }

    /// <summary>
    /// Kanban Group model
    /// </summary>
    public class KanbanGroupModel
    {
        // Unique Group ID
        public string id { get; set; }

        // Group's internal object id
        public string objId { get; set; }

        // Group display name
        public string name { get; set; }

        // Progress
        public int progress { get; set; }
        //Track
        public bool track { get; set; }

        // group's db class name
        public string className { get; set; }

        // Items's write permission
        public bool allowWrite { get; set; }

        // group commands
        public List<string> commands { get; set; }
    }

    /// <summary>
    /// Kanban Item model
    /// </summary>
    public class KanbanItemModel
    {
        // Unique Group ID
        public string id { get; set; }

        // item's internal object id
        public string objId { get; set; }

        // Group display name
        public string name { get; set; }

        public string groupId { get; set; }

        // State
        public string stateId { get; set; }

        // Items's db class name
        public string className { get; set; }

        // Items's write permission
        public bool allowWrite { get; set; }

        // item commands
        public List<string> commands { get; set; }
    }

    public class KanbanCommandModel
    {
        // Command id
        public string id { get; set; }

        // Command title
        public string title { get; set; }

        // command icon
        public string icon { get; set; }

        // command url
        public string url { get; set; }

        // command parameter
        public List<CommandParameterModel> parameters { get; set; }
    }

    public class CommandParameterModel
    {
        // command parameter name
        public string name {get; set; }

        // Command parameter value
        public string value { get; set; }
    }


    public class KanbanModel
    {
        public string text { get; set; }

        public List<KanbanStateModel> states { get; set; }

        public List<KanbanGroupModel> groups { get; set; }

        public List<KanbanItemModel> items { get; set; }

        public List<KanbanCommandModel> groupCommands { get; set; }

        public List<KanbanCommandModel> itemCommands { get; set; }
    }
}