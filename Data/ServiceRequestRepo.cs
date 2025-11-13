using System.Xml.Serialization;
using MunicipalServicesApp.Model;

namespace MunicipalServicesApp.Data
{
    public class ServiceRequestRepo
    {
        private readonly string filePath;             // full path to the XML file on disk
        private List<ServiceRequest> items = new List<ServiceRequest>(); // in-memory list of requests

        //-------------------------------------------------- Constructor --------------------------------------------------//

        // Create the repository and try to load existing requests from disk.
        // Default file name is service_requests.xml stored in the application's base folder.
        public ServiceRequestRepo(string fileName = "service_requests.xml")
        {
            // store file in application folder so it's easy to find
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            Load();
        }

        //---------------------------------------------------- Public API --------------------------------------------------//

        // Read-only view of all requests
        public IReadOnlyList<ServiceRequest> All => items.AsReadOnly();

        // Add a new request and save immediately
        public void Add(ServiceRequest r)
        {
            if (r == null) return;
            items.Add(r);
            Save();
        }

        // Find a request by id (returns null when not found)
        public ServiceRequest GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return items.Find(x => x.Id == id);
        }

        // Update an existing request (matches by Id) and save changes
        public void Update(ServiceRequest updated)
        {
            if (updated == null) return;

            int idx = items.FindIndex(x => x.Id == updated.Id);
            if (idx >= 0)
            {
                items[idx] = updated;
                Save();
            }
        }

        //-------------------------------------------------- Persistence --------------------------------------------------//

        // Save the in-memory list to an XML file.
        // Keeps data between runs. Shows a MessageBox on failure.
        public void Save()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<ServiceRequest>));
                using var stream = File.Create(filePath);
                serializer.Serialize(stream, items);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving requests: " + ex.Message,
                    "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load the list from the XML file. If file doesn't exist or is invalid, start with an empty list.
        public void Load()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    items = new List<ServiceRequest>();
                    return;
                }

                var serializer = new XmlSerializer(typeof(List<ServiceRequest>));
                using var stream = File.OpenRead(filePath);
                items = (List<ServiceRequest>)serializer.Deserialize(stream) ?? new List<ServiceRequest>();
            }
            catch
            {
                // If anything goes wrong (corrupt file, etc.), reset to an empty list to avoid crashes.
                items = new List<ServiceRequest>();
            }
        }
    }
}