# План исправления критических багов и архитектурных проблем

## Этап 1: Внедрение системы резервирования (ReservedStock)
*   **Проблема:** Прямое списание при создании заказа создает сложности с отменой и просрочками.
*   **Действие:**
    *   Обновить `Product`: добавить поле `ReservedQuantity`.
    *   Реализовать методы `ReserveStock(int quantity)` и `CommitReservation(int quantity)`.
    *   Обновить `OrderService`: при создании заказа вызывать `ReserveStock`.
    *   Обновить `OrderService.CancelOrderAsync` и создать `OrderExpirationService`: вызывать логику отмены резерва.
    *   Реализовать `PaymentSucceededEventHandler`: при успешной оплате вызывать `CommitReservation` (реальное списание).

## Этап 2: Надежная обработка событий (Transactional Outbox)
*   **Действие:**
    *   Создать сущность `OutboxMessage` и добавить `DbSet` в `ApplicationDbContext`.
    *   Обновить `DomainEventPublishingInterceptor` для сохранения событий в `OutboxMessage` в рамках той же транзакции.
    *   Реализовать `OutboxProcessor` (BackgroundService) для публикации событий.

## Этап 3: Доменные события и возврат на склад
*   **Действие:**
    *   Реализовать `ReturnProductToStockHandler` (Domain Event Handler для `RefundSucceededEvent`), который возвращает товар на склад (через `AddToStock`).

## Этап 4: Исправления и очистка
*   **Возврат:** Исправить логику `isFullRefund`.
*   **Оптимизация:** Удалить лишние `SaveChangesAsync`.
*   **Абстракции:** Убрать `dynamic` (выполнено), перенести специфичные команды (в процессе).
