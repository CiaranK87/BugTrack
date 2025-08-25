import { Button, Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import ProjectListItem from "./ProjectListItem";
import { Fragment } from "react";
import { NavLink } from "react-router-dom";

export default observer(function ProjectList() {
  const { projectStore } = useStore();
  const { projectsByStartDate } = projectStore;

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <Header sub color="teal">ACTIVE PROJECTS</Header>
        <Button
          as={NavLink}
          to="/createProject"
          basic
          color="teal"
          content="Create Project"
          size="small"
        />
      </div>
      {projectsByStartDate.map((project) => (
        <Fragment key={project.id}>
          <ProjectListItem key={project.id} project={project} />
        </Fragment>
      ))}
    </>
  );
});

