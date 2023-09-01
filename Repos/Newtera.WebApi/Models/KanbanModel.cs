using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Newtera.WebApi.Models
{
    /// <summary>
    /// Kanban State model
    /// </summary>
    public class KanbanStateModel
    {
        public KanbanStateModel(string id, string stateName)
        {
            this.Id = id;
            this.Name = stateName;
        }

        public string Id { get; }

        public string Name { get;}

        // Whether to allow adding item to this state
        public bool AreNewItemButtonsHidden { get; set; }
    }

    /// <summary>
    /// Kanban Group model
    /// </summary>
    public class KanbanGroupModel
    {
        public KanbanGroupModel(string id, string groupName)
        {
            this.Id = id;
            this.Name = groupName;
            this.Commands = new List<string>();
        }

        public string Id { get; }
        public string Name { get;}
        public bool AllowWrite { get; set; }
        public List<string> Commands { get; set; }
    }

    /// <summary>
    /// Kanban Item model
    /// </summary>
    public class KanbanItemModel
    {
        public KanbanItemModel()
        {
            this.Commands = new List<string>();
        }

        public string ObjId { get; set; }
        public string Title { get; set; }

        public string Name { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string StateId { get; set; }
        public string StateName { get; set; }
        public bool Track { get; set; }
        // Items's write permission
        public bool AllowWrite { get; set; }
        // item commands
        public List<string> Commands { get; set; }
    }

    public class KanbanModel
    {
        public KanbanModel()
        {
            this.Groups = new List<KanbanGroupModel>();
            this.States = new List<KanbanStateModel>();
            this.Items = new List<KanbanItemModel>();
        }
        public IList<KanbanGroupModel> Groups { get;}
        public IList<KanbanStateModel> States { get; }
        public IList<KanbanItemModel> Items { get; }
    }
}