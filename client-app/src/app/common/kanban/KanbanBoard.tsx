import React, { useState } from 'react';
import { Grid, Header, Segment, Dropdown } from 'semantic-ui-react';
import {
  DndContext,
  DragOverlay,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragStartEvent,
  DragEndEvent,
  DragOverEvent,
  useDroppable,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import {
  useSortable,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

interface KanbanColumn {
  id: string;
  title: string;
  items: any[];
  color?: string;
}

interface KanbanBoardProps {
  columns: KanbanColumn[];
  onDragEnd: (columns: KanbanColumn[]) => void;
  renderItem: (item: any, index: number) => React.ReactNode;
  loading?: boolean;
  title?: string;
  showViewToggle?: boolean;
  onViewChange?: (view: string) => void;
  currentView?: string;
  viewOptions?: Array<{ key: string; text: string; value: string }>;
}

const SortableItem: React.FC<{ id: string; children: React.ReactNode }> = ({ id, children }) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
    >
      {children}
    </div>
  );
};

const DroppableColumn: React.FC<{
  id: string;
  children: React.ReactNode;
  title: string;
  color?: string;
  itemCount: number;
}> = ({ id, children, title, color, itemCount }) => {
  const { setNodeRef, isOver } = useDroppable({ id });

  return (
    <Grid.Column>
      <Segment
        ref={setNodeRef}
        style={{
          minHeight: '500px',
          backgroundColor: isOver ? '#e8f4fd' : '#f8f9fa',
          padding: '0',
          border: isOver ? '2px dashed #007bff' : 'none'
        }}
      >
        <Header as="h3" color={(color as any) || 'grey'} textAlign="center" style={{ padding: '15px 0 10px 0' }}>
          {title}
          <Header.Subheader>
            {itemCount} item{itemCount !== 1 ? 's' : ''}
          </Header.Subheader>
        </Header>
        
        <div
          style={{
            padding: '8px',
            minHeight: '400px'
          }}
        >
          {children}
        </div>
      </Segment>
    </Grid.Column>
  );
};

const KanbanBoard: React.FC<KanbanBoardProps> = ({
  columns,
  onDragEnd,
  renderItem,
  loading = false,
  title,
  showViewToggle = false,
  onViewChange,
  currentView,
  viewOptions = []
}) => {
  const [activeId, setActiveId] = useState<string | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragStart = (event: DragStartEvent) => {
    setActiveId(event.active.id as string);
  };

  const handleDragOver = (event: DragOverEvent) => {
    const { over } = event;
    if (!over) return;
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (!over) {
      setActiveId(null);
      return;
    }

    const activeId = active.id as string;
    const overId = over.id as string;

    let activeItem: any = null;
    let sourceColumnIndex = -1;
    let sourceItemIndex = -1;

    columns.forEach((column, colIndex) => {
      const itemIndex = column.items.findIndex(item => item.id === activeId);
      if (itemIndex !== -1) {
        activeItem = column.items[itemIndex];
        sourceColumnIndex = colIndex;
        sourceItemIndex = itemIndex;
      }
    });

    if (!activeItem) {
      setActiveId(null);
      return;
    }

    const newColumns = [...columns];

    const dropOnColumnIndex = columns.findIndex(col => col.id === overId);
    
    if (dropOnColumnIndex !== -1) {
      if (sourceColumnIndex === dropOnColumnIndex) {
        setActiveId(null);
        return;
      }

      newColumns[sourceColumnIndex] = {
        ...newColumns[sourceColumnIndex],
        items: newColumns[sourceColumnIndex].items.filter(item => item.id !== activeId)
      };

      newColumns[dropOnColumnIndex] = {
        ...newColumns[dropOnColumnIndex],
        items: [...newColumns[dropOnColumnIndex].items, activeItem]
      };

      onDragEnd(newColumns);
      setActiveId(null);
      return;
    }

    let dropOnItemColumnIndex = -1;
    let dropOnItemIndex = -1;

    columns.forEach((column, colIndex) => {
      const itemIndex = column.items.findIndex(item => item.id === overId);
      if (itemIndex !== -1) {
        dropOnItemColumnIndex = colIndex;
        dropOnItemIndex = itemIndex;
      }
    });

    if (dropOnItemColumnIndex === -1) {
      setActiveId(null);
      return;
    }

    if (sourceColumnIndex === dropOnItemColumnIndex) {
      newColumns[sourceColumnIndex] = {
        ...newColumns[sourceColumnIndex],
        items: arrayMove(newColumns[sourceColumnIndex].items, sourceItemIndex, dropOnItemIndex)
      };
    } else {
      newColumns[sourceColumnIndex] = {
        ...newColumns[sourceColumnIndex],
        items: newColumns[sourceColumnIndex].items.filter(item => item.id !== activeId)
      };
      
      const targetItems = [...newColumns[dropOnItemColumnIndex].items];
      targetItems.splice(dropOnItemIndex, 0, activeItem);
      
      newColumns[dropOnItemColumnIndex] = {
        ...newColumns[dropOnItemColumnIndex],
        items: targetItems
      };
    }

    onDragEnd(newColumns);
    setActiveId(null);
  };

  const activeItem = activeId 
    ? columns.flatMap(col => col.items).find(item => item.id === activeId)
    : null;

  return (
    <div style={{ padding: '20px' }}>
      {title && (
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <Header as="h2" color="teal">{title}</Header>
          {showViewToggle && (
            <Dropdown
              selection
              options={viewOptions}
              value={currentView}
              onChange={(_, data) => onViewChange?.(data.value as string)}
              style={{ minWidth: '150px' }}
            />
          )}
        </div>
      )}
      
      {loading ? (
        <Segment loading>
          <div style={{ height: '400px' }}></div>
        </Segment>
      ) : (
        <DndContext
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragStart={handleDragStart}
          onDragOver={handleDragOver}
          onDragEnd={handleDragEnd}
        >
          <Grid columns={Math.min(columns.length, 4) as any} stackable>
            {columns.map((column) => (
              <DroppableColumn
                key={column.id}
                id={column.id}
                title={column.title}
                color={column.color}
                itemCount={column.items.length}
              >
                <SortableContext
                  items={column.items.map(item => item.id)}
                  strategy={verticalListSortingStrategy}
                >
                  {column.items.map((item, index) => (
                    <SortableItem key={item.id} id={item.id}>
                      <div style={{ marginBottom: '8px' }}>
                        {renderItem(item, index)}
                      </div>
                    </SortableItem>
                  ))}
                </SortableContext>
              </DroppableColumn>
            ))}
          </Grid>
          
          <DragOverlay>
            {activeItem ? (
              <div style={{ opacity: 0.8, transform: 'rotate(5deg)' }}>
                {renderItem(activeItem, 0)}
              </div>
            ) : null}
          </DragOverlay>
        </DndContext>
      )}
    </div>
  );
};

export default KanbanBoard;