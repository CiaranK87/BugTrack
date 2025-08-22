import { observer } from "mobx-react-lite";
import { ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip } from 'recharts';
import { Segment, Header } from "semantic-ui-react";

interface Props {
  data: Array<{ name: string; count: number }>;
}

export default observer(function TicketPriorityChart({ data }: Props) {
  return (
    <Segment>
      <Header as="h3">Ticket Priority</Header>
      <ResponsiveContainer width="100%" height={200}>
        <BarChart data={data} margin={{ top: 10, right: 20, left: 10, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="name" />
          <YAxis domain={[0, 'dataMax']} allowDecimals={false} />
          <Tooltip />
          <Bar dataKey="count" fill="#2185d0" />
        </BarChart>
      </ResponsiveContainer>
    </Segment>
  );
});