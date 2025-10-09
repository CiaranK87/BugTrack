import { Button, Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import ProjectListItem from "./ProjectListItem";
import { NavLink } from "react-router-dom";

export default observer(function ProjectList() {
  const { projectStore, userStore } = useStore();
  const { projectsByStartDate } = projectStore;
  const { canCreateProjects } = userStore;

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Header sub color="teal" size="huge">ACTIVE PROJECTS</Header>
        {canCreateProjects && (
          <Button as={NavLink} to="/createProject" basic color="teal" content="Create Project" size="small"/>
        )}
      </div>
      {projectsByStartDate.map((project) => (
          <ProjectListItem key={project.id} project={project} />
      ))}
    </>
  );
});

