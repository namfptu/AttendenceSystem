document.addEventListener("DOMContentLoaded", function () {
    const tables = document.querySelectorAll("table#datatable-buttons, table.js-paginate");
    tables.forEach((table, index) => {
        initializePagination(table, index);
    });

    function initializePagination(table, index) {
        const tbody = table.querySelector("tbody");
        if (!tbody) return;

        // Get all original rows
        const originalRows = Array.from(tbody.querySelectorAll("tr"));
        if (originalRows.length === 0 || (originalRows.length === 1 && originalRows[0].cells.length === 1 && originalRows[0].textContent.trim().includes("No"))) {
            return; // Empty table
        }

        let currentPage = 1;
        let pageSize = 10;
        let filteredRows = [...originalRows];

        // Create Unique IDs for controls of this specific table
        const pageSizeId = `dt-page-size-${index}`;
        const searchId = `dt-search-${index}`;
        const infoId = `dt-info-${index}`;
        const paginationId = `dt-pagination-${index}`;

        // Create Top Controls Container (Search & Entries)
        const topRow = document.createElement("div");
        topRow.className = "row mb-3 align-items-center";
        topRow.innerHTML = `
            <div class="col-sm-6 d-flex align-items-center gap-2 mb-2 mb-sm-0">
                <span class="text-muted small">Show</span>
                <select class="form-select form-select-sm d-inline-block" style="width: auto;" id="${pageSizeId}">
                    <option value="5">5</option>
                    <option value="10" selected>10</option>
                    <option value="20">20</option>
                    <option value="50">50</option>
                </select>
                <span class="text-muted small">entries</span>
            </div>
            <div class="col-sm-6 d-flex justify-content-sm-end align-items-center gap-2">
                <span class="text-muted small">Search:</span>
                <input type="search" class="form-control form-control-sm" style="width: 200px;" id="${searchId}" placeholder="Type to filter...">
            </div>
        `;

        // Insert Top Controls before the table
        table.parentNode.insertBefore(topRow, table);

        // Create Bottom Controls Container (Info & Pagination)
        const bottomRow = document.createElement("div");
        bottomRow.className = "row mt-3 align-items-center";
        bottomRow.innerHTML = `
            <div class="col-sm-6 mb-2 mb-sm-0">
                <div class="text-muted small" id="${infoId}"></div>
            </div>
            <div class="col-sm-6 d-flex justify-content-sm-end">
                <nav aria-label="Table pagination">
                    <ul class="pagination pagination-sm mb-0" id="${paginationId}"></ul>
                </nav>
            </div>
        `;

        // Insert Bottom Controls after the table
        table.parentNode.insertBefore(bottomRow, table.nextSibling);

        const pageSizeSelect = document.getElementById(pageSizeId);
        const searchInput = document.getElementById(searchId);
        const infoDiv = document.getElementById(infoId);
        const paginationUl = document.getElementById(paginationId);

        function renderTable() {
            const totalEntries = filteredRows.length;
            const totalPages = Math.ceil(totalEntries / pageSize) || 1;

            if (currentPage > totalPages) {
                currentPage = totalPages;
            }

            const startIndex = (currentPage - 1) * pageSize;
            const endIndex = Math.min(startIndex + pageSize, totalEntries);

            // Hide all original rows first
            originalRows.forEach(row => row.style.display = "none");

            // Show only rows in current page range
            for (let i = startIndex; i < endIndex; i++) {
                filteredRows[i].style.display = "";
            }

            // Update info text
            if (totalEntries === 0) {
                infoDiv.textContent = "Showing 0 to 0 of 0 entries";
                // Show a temporary "No matching records found" row
                tbody.innerHTML = `<tr><td colspan="${table.rows[0].cells.length}" class="text-center text-muted py-4">No matching records found.</td></tr>`;
            } else {
                // Restore tbody structure if empty row was added
                const emptyRow = tbody.querySelector(".text-center.text-muted");
                if (emptyRow) {
                    tbody.innerHTML = "";
                    originalRows.forEach(row => tbody.appendChild(row));
                }
                infoDiv.textContent = `Showing ${startIndex + 1} to ${endIndex} of ${totalEntries} entries` + (filteredRows.length !== originalRows.length ? ` (filtered from ${originalRows.length} total entries)` : "");
            }

            // Render Pagination buttons
            renderPagination(totalPages);
        }

        function renderPagination(totalPages) {
            paginationUl.innerHTML = "";

            // Previous button
            const prevLi = document.createElement("li");
            prevLi.className = `page-item ${currentPage === 1 ? "disabled" : ""}`;
            prevLi.innerHTML = `<a class="page-link" href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a>`;
            prevLi.addEventListener("click", function (e) {
                e.preventDefault();
                if (currentPage > 1) {
                    currentPage--;
                    renderTable();
                }
            });
            paginationUl.appendChild(prevLi);

            // Page numbers
            const maxVisibleButtons = 5;
            let startPage = Math.max(1, currentPage - Math.floor(maxVisibleButtons / 2));
            let endPage = Math.min(totalPages, startPage + maxVisibleButtons - 1);

            if (endPage - startPage + 1 < maxVisibleButtons) {
                startPage = Math.max(1, endPage - maxVisibleButtons + 1);
            }

            for (let i = startPage; i <= endPage; i++) {
                const pageLi = document.createElement("li");
                pageLi.className = `page-item ${i === currentPage ? "active" : ""}`;
                pageLi.innerHTML = `<a class="page-link" href="#">${i}</a>`;
                pageLi.addEventListener("click", function (e) {
                    e.preventDefault();
                    currentPage = i;
                    renderTable();
                });
                paginationUl.appendChild(pageLi);
            }

            // Next button
            const nextLi = document.createElement("li");
            nextLi.className = `page-item ${currentPage === totalPages ? "disabled" : ""}`;
            nextLi.innerHTML = `<a class="page-link" href="#" aria-label="Next"><span aria-hidden="true">&raquo;</span></a>`;
            nextLi.addEventListener("click", function (e) {
                e.preventDefault();
                if (currentPage < totalPages) {
                    currentPage++;
                    renderTable();
                }
            });
            paginationUl.appendChild(nextLi);
        }

        // Event listeners
        pageSizeSelect.addEventListener("change", function () {
            pageSize = parseInt(this.value);
            currentPage = 1;
            renderTable();
        });

        searchInput.addEventListener("input", function () {
            const query = this.value.trim().toLowerCase();
            currentPage = 1;

            if (query === "") {
                filteredRows = [...originalRows];
            } else {
                filteredRows = originalRows.filter(row => {
                    return row.textContent.toLowerCase().includes(query);
                });
            }

            renderTable();
        });

        // Initial render
        renderTable();
    }
});
