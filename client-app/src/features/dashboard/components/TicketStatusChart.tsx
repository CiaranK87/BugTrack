import { observer } from "mobx-react-lite";
import { ResponsiveContainer, PieChart, Pie, Cell, Tooltip, Legend } from 'recharts';
import { Segment, Header } from "semantic-ui-react";

interface Props {
  data: Array<{ name: string; value: number; color: string }>;
}

export default observer(function TicketStatusChart({ data }: Props) {
  return (
    <Segment>
      <Header as="h3">Ticket Status</Header>
      <ResponsiveContainer width="100%" height={250} className="responsive-pie-chart">
        <PieChart>
          <Pie
            data={data.filter(d => d.value > 0)}
            cx="50%"
            cy="50%"
            outerRadius={80}
            dataKey="value"
            label={({ name, value }) => `${name}: ${value}`}
            isAnimationActive={false}
          >
            {data.map((entry, index) => (
              <Cell key={`cell-status-${index}`} fill={entry.color} />
            ))}
          </Pie>
          <Tooltip />
          <Legend verticalAlign="bottom" height={36} />
        </PieChart>
      </ResponsiveContainer>
    </Segment>
  );
});