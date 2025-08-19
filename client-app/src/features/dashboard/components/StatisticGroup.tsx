// features/dashboard/components/StatisticGroup.tsx
import { observer } from "mobx-react-lite";
import { Statistic, Grid, Icon } from "semantic-ui-react";

interface Props {
  totalProjects: number;
  activeProjects: number;
  totalTickets: number;
  openTickets: number;
}

export default observer(function StatisticGroup({
  totalProjects,
  activeProjects,
  totalTickets,
  openTickets
}: Props) {
  return (
    <Grid columns={4} style={{ marginBottom: '1em' }}>
      <Grid.Column textAlign="center">
        <Statistic size="small">
          <Statistic.Value>{totalProjects}</Statistic.Value>
          <Statistic.Label>
            <Icon name="folder outline" size="small" style={{ marginRight: '0.5em' }} />
            Projects
          </Statistic.Label>
        </Statistic>
      </Grid.Column>
      <Grid.Column textAlign="center">
        <Statistic size="small" color="green">
          <Statistic.Value>{activeProjects}</Statistic.Value>
          <Statistic.Label>
            <Icon name="play circle outline" size="small" style={{ marginRight: '0.5em' }} />
            Active
          </Statistic.Label>
        </Statistic>
      </Grid.Column>
      <Grid.Column textAlign="center">
        <Statistic size="small">
          <Statistic.Value>{totalTickets}</Statistic.Value>
          <Statistic.Label>
            <Icon name="list ul" size="small" style={{ marginRight: '0.5em' }} />
            Tickets
          </Statistic.Label>
        </Statistic>
      </Grid.Column>
      <Grid.Column textAlign="center">
        <Statistic size="small" color="blue">
          <Statistic.Value>{openTickets}</Statistic.Value>
          <Statistic.Label>
            <Icon name="check circle outline" size="small" style={{ marginRight: '0.5em' }} />
            Open
          </Statistic.Label>
        </Statistic>
      </Grid.Column>
    </Grid>
  );
});