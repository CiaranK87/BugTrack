import { observer } from "mobx-react-lite";
import { ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Cell } from 'recharts';
import { Segment, Header } from "semantic-ui-react";

interface Props {
  data: Array<{ name: string; count: number }>;
}

const getSeverityColor = (severity: string) => {
  switch (severity) {
    case 'Critical': return '#ff6b6b';
    case 'High': return '#ffa94d';
    case 'Medium': return '#ffd43b';
    case 'Low': return '#8ce99a';
    default: return '#adb5bd';
  }
};

export default observer(function TicketSeverityChart({ data }: Props) {
  return (
    <Segment>
      <Header as="h3">Ticket Severity</Header>
      <ResponsiveContainer width="100%" height={200}>
        <BarChart data={data} margin={{ top: 10, right: 20, left: 10, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="name" />
          <YAxis domain={[0, 'dataMax']} allowDecimals={false} />
          <Tooltip />
          <Bar dataKey="count">
            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={getSeverityColor(entry.name)} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </Segment>
  );
});