import { useEffect, useState } from "react";
import "./App.css";
import axios from "axios";
import { Header, List } from "semantic-ui-react";

function App() {
  const [projects, setProjects] = useState([]);

  useEffect(() => {
    axios.get("http://localhost:5000/api/projects").then((response) => {
      setProjects(response.data);
    });
  }, []);

  return (
    <div>
      <Header as="h2" icon="group" content="BugTrack" />
      <List>
        {projects.map((project: any) => (
          <List.Item key={project.id}>{project.name}</List.Item>
        ))}
      </List>
    </div>
  );
}

export default App;
