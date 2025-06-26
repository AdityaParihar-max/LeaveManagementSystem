const token = localStorage.getItem("jwtToken");
if (!localStorage.getItem("email") || localStorage.getItem("isAdmin")) {
  window.location.href = "employee-login.html";
}

const toggle = document.getElementById("toggle");
const nav = document.getElementById("nav");

toggle.addEventListener("click", () => nav.classList.toggle("active"));

document.addEventListener("DOMContentLoaded", async () => {
  const email = localStorage.getItem("email");
  if (!email) {
    window.location.href = "employee-login.html";
    return;
  }

  const container = document.getElementById("leavesContainer");
  const pagination = document.getElementById("paginationControls");
  const itemsPerPageSelect = document.getElementById("itemsPerPage");

  let currentPage = 1;
  let leavesPerPage = parseInt(itemsPerPageSelect.value);
  let leaves = [];

  try {
    const response = await fetch(
      `http://localhost:5000/api/Search/email/${email}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );
    leaves = await response.json();
    renderLeaves();
    renderPagination();
  } catch (error) {
    console.error("Error fetching leaves:", error);
  }

  itemsPerPageSelect.addEventListener("change", () => {
    leavesPerPage = parseInt(itemsPerPageSelect.value);
    currentPage = 1;
    renderLeaves();
    renderPagination();
  });

  function renderLeaves() {
    container.innerHTML = "";

    const start = (currentPage - 1) * leavesPerPage;
    const paginatedLeaves = leaves.slice(start, start + leavesPerPage);

    paginatedLeaves.forEach((leave) => {
      const entry = document.createElement("div");
      entry.className = "leave-entry";
      entry.innerHTML = `
        <div>${leave.startDateFormatted}</div>
        <div>${leave.endDateFormatted}</div>
        <div>${leave.reason}</div>
        <div class="status ${leave.status}">${leave.status}</div>
      `;
      container.appendChild(entry);
    });
  }

  function renderPagination() {
    pagination.innerHTML = "";
    const totalPages = Math.ceil(leaves.length / leavesPerPage);

    if (currentPage > 1) {
      const prevBtn = document.createElement("button");
      prevBtn.textContent = "Previous";
      prevBtn.onclick = () => {
        currentPage--;
        renderLeaves();
        renderPagination();
      };
      pagination.appendChild(prevBtn);
    }

    for (let i = 1; i <= totalPages; i++) {
      const pageBtn = document.createElement("button");
      pageBtn.textContent = i;
      if (i === currentPage) pageBtn.classList.add("active");
      pageBtn.onclick = () => {
        currentPage = i;
        renderLeaves();
        renderPagination();
      };
      pagination.appendChild(pageBtn);
    }

    if (currentPage < totalPages) {
      const nextBtn = document.createElement("button");
      nextBtn.textContent = "Next";
      nextBtn.onclick = () => {
        currentPage++;
        renderLeaves();
        renderPagination();
      };
      pagination.appendChild(nextBtn);
    }
  }
});

//----------------------------------------------------------------------------------------

document.getElementById("logoutBtn").addEventListener("click", function (e) {
  e.preventDefault();
  localStorage.clear();
  // Redirect to the appropriate login page

  window.location.href = "employee-login.html";
});
