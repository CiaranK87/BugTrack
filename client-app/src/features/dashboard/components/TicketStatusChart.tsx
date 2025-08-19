// features/dashboard/components/TicketStatusChart.tsx
import { observer } from "mobx-react-lite";
import { ResponsiveContainer, PieChart, Pie, Cell, Tooltip } from 'recharts';
import { Segment, Header } from "semantic-ui-react";

interface Props {
  data: Array<{ name: string; value: number; color: string }>;
}

export default observer(function TicketStatusChart({ data }: Props) {
  return (
    <Segment>
      <Header as="h3">Ticket Status</Header>
      <ResponsiveContainer width="100%" height={200}>
        <PieChart>
          <Pie
            data={data}
            cx="50%"
            cy="50%"
            outerRadius={60}
            dataKey="value"
            label={({ name, value }) => `${name}: ${value}`}
          >
            {data.map((entry, index) => (
              <Cell key={`cell-status-${index}`} fill={entry.color} />
            ))}
          </Pie>
          <Tooltip />
        </PieChart>
      </ResponsiveContainer>
    </Segment>
  );
});