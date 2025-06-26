const token = localStorage.getItem("jwtToken");
const role = localStorage.getItem("role");

if (!token || role !== "Admin") {
  alert("Unauthorized access.");
  window.location.href = "admin-login.html";
}

const toggle = document.getElementById("toggle");
const nav = document.getElementById("nav");
toggle.addEventListener("click", () => nav.classList.toggle("active"));

const leaveList = document.getElementById("leaveList");
const paginationControls = document.getElementById("paginationControls");
const itemsPerPageSelect = document.getElementById("itemsPerPageSelect");

let currentPage = 1;
let itemsPerPage = parseInt(itemsPerPageSelect.value);
let pendingLeaves = [];

itemsPerPageSelect.addEventListener("change", () => {
  itemsPerPage = parseInt(itemsPerPageSelect.value);
  currentPage = 1;
  renderPage();
});

async function fetchLeaves() {
  try {
    const res = await fetch("http://localhost:5000/api/admin/pending", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (!res.ok) throw new Error("Failed to fetch pending leaves");

    pendingLeaves = await res.json();
    if (!Array.isArray(pendingLeaves)) pendingLeaves = [];

    const totalPages = Math.ceil(pendingLeaves.length / itemsPerPage);
    if (currentPage > totalPages) currentPage = totalPages || 1;

    renderPage();
  } catch (err) {
    console.error("Error fetching leaves:", err);
    leaveList.innerHTML = "<p>Error loading pending leaves.</p>";
    paginationControls.innerHTML = "";
  }
}

function renderPage() {
  leaveList.innerHTML = "";

  if (pendingLeaves.length === 0) {
    leaveList.innerHTML = "<p>No pending leaves found.</p>";
    paginationControls.innerHTML = "";
    return;
  }

  const startIndex = (currentPage - 1) * itemsPerPage;
  const leavesToDisplay = pendingLeaves.slice(
    startIndex,
    startIndex + itemsPerPage
  );

  leavesToDisplay.forEach((leave) => {
    const div = document.createElement("div");
    div.classList.add("leave-item");
    div.innerHTML = `
      <p><strong>Email:</strong> ${leave.email}</p>
      <p><strong>Leave Type:</strong> ${leave.leaveType}</p>
      <p><strong>Start Date:</strong> ${leave.startDateFormatted}</p>
      <p><strong>End Date:</strong> ${leave.endDateFormatted}</p>
      <p><strong>Reason:</strong> ${leave.reason}</p>
      <p><strong>Status:</strong> ${leave.status}</p>
      <button class="approve-btn" data-id="${leave.id}">Approve</button>
      <button class="reject-btn" data-id="${leave.id}">Reject</button>
    `;
    leaveList.appendChild(div);
  });

  document
    .querySelectorAll(".approve-btn")
    .forEach((btn) =>
      btn.addEventListener("click", () =>
        updateStatus(btn.dataset.id, "Approved")
      )
    );
  document
    .querySelectorAll(".reject-btn")
    .forEach((btn) =>
      btn.addEventListener("click", () =>
        updateStatus(btn.dataset.id, "Rejected")
      )
    );

  renderPagination();
}

function renderPagination() {
  paginationControls.innerHTML = "";

  const totalPages = Math.ceil(pendingLeaves.length / itemsPerPage);
  if (totalPages === 0) return;

  // Prev Button
  if (currentPage > 1) {
    const prevBtn = document.createElement("button");
    prevBtn.innerHTML = `<i class="fas fa-arrow-left"></i> Prev`;
    prevBtn.onclick = () => {
      currentPage--;
      renderPage();
    };
    paginationControls.appendChild(prevBtn);
  }

  // Page Number Buttons
  for (let i = 1; i <= totalPages; i++) {
    const pageBtn = document.createElement("button");
    pageBtn.textContent = i;
    if (i === currentPage) pageBtn.style.backgroundColor = "#004aad";
    pageBtn.onclick = () => {
      currentPage = i;
      renderPage();
    };
    paginationControls.appendChild(pageBtn);
  }

  // Next Button
  if (currentPage < totalPages) {
    const nextBtn = document.createElement("button");
    nextBtn.innerHTML = `Next <i class="fas fa-arrow-right"></i>`;
    nextBtn.onclick = () => {
      currentPage++;
      renderPage();
    };
    paginationControls.appendChild(nextBtn);
  }
}

async function updateStatus(leaveId, status) {
  try {
    const res = await fetch(
      `http://localhost:5000/api/Admin/${leaveId}/status`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ status }),
      }
    );

    if (!res.ok) throw new Error("Failed to update status");

    alert(`Leave ${status} successfully.`);
    await fetchLeaves(); // Refresh the list
  } catch (err) {
    console.error("Error updating status:", err);
    alert("Failed to update leave status.");
  }
}

fetchLeaves();

//------------------------------------------------------------------------------------
document.getElementById("logoutBtn").addEventListener("click", function (e) {
  e.preventDefault();
  localStorage.clear();
  // Redirect to the appropriate login page
  window.location.href = "admin-login.html";
});
