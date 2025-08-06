import { Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import { observer } from "mobx-react-lite";
import ProjectListItem from "./ProjectListItem";
import { Fragment } from "react";

export default observer(function ProjectList() {
  const { projectStore } = useStore();
  const { projectsByStartDate } = projectStore;

  return (
    <>
      <Header sub color="teal">
        ACTIVE PROJECTS
      </Header>
      {projectsByStartDate.map((project) => (
        <Fragment key={project.id}>
          <ProjectListItem key={project.id} project={project} />
        </Fragment>
      ))}
    </>
  );
});

